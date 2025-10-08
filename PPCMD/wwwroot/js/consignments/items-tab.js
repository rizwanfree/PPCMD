// items-tab.js

let itemIndex = 0;

function initializeItemsTab() {
    addItemRow(); // Add first item row by default

    $('#addItem').click(function () {
        addItemRow();
    });

    $(document).on('click', '.remove-item', function () {
        $(this).closest('.item-row').remove();
    });

    // Add exchange rate change listener
    $('[name="PendingBL.BL.LC.ExchangeRate"]').on('input', function () {
        $('.item-row').each(function () {
            const itemIndex = $(this).index();
            calculateItemValues($(this).find('.quantity-input')[0], itemIndex);
        });
    });

    // Listen for duty rate changes
    $(document).on('input', '.duty-rate-input', function () {
        const itemRow = $(this).closest('.item-row');
        const assessableValue = parseFloat(itemRow.find('.assessable-value').val()) || 0;
        updateDutyCalculations(itemRow, assessableValue);
    });
}

function addItemRow() {
    const index = itemIndex++;
    const itemRow = `
        <div class="item-row border p-2 mb-1">
            <div class="row g-1 align-items-center">
                <div class="col-2">
                    <label class="form-label">Item</label>
                    <select name="Items[${index}].ItemId" class="form-select form-select-sm item-select" onchange="loadItemDuties(this, ${index})">
                        <option value="">-- Select Item --</option>
                    </select>
                </div>
                <div class="col-1">
                    <label class="form-label">Quantity</label>
                    <input type="number" name="Items[${index}].Quantity" class="form-control form-control-sm text-end quantity-input" step="0.01" oninput="calculateItemValues(this, ${index})" />
                </div>
                <div class="col-1">
                    <label class="form-label">Unit Value $</label>
                    <input type="number" name="Items[${index}].UnitValue" class="form-control form-control-sm text-end unit-value-input" step="0.01" oninput="calculateItemValues(this, ${index})" />
                </div>
                <div class="col-2">
                    <label class="form-label">Invoice Value $</label>
                    <input type="number" name="Items[${index}].ImportValue" class="form-control form-control-sm text-end import-value-usd" step="0.01" readonly/>
                </div>
                <div class="col-2">
                    <label class="form-label">Invoice Value PK</label>
                    <input type="number" name="Items[${index}].InvoiceValuePKR" class="form-control form-control-sm text-end import-value-pkr" step="0.01" readonly/>
                </div>

                <div class="col-1">
                    <label class="form-label">Insurance $</label>
                    <input type="number" name="Items[${index}].InsuranceValue" class="form-control form-control-sm text-end insurance-usd" step="0.01" oninput="calculateItemValues(this, ${index})" />
                </div>
                <div class="col-1">
                    <label class="form-label">Insurance PKR</label>
                    <input type="number" name="Items[${index}].InsuranceValuePKR" class="form-control form-control-sm text-end insurance-pkr" step="0.01" readonly/>
                </div>

                <div class="col-1">
                    <label class="form-label">Landing Ch</label>
                    <input type="number" name="Items[${index}].LandingCharges" class="form-control form-control-sm text-end landing-charges" step="0.01" readonly/>
                </div>
                <div class="col-md-1">
                    <label class="form-label">Import Value</label>
                    <input type="number" name="Items[${index}].AssessableValue" class="form-control form-control-sm text-end assessable-value" step="0.01" readonly/>
                </div>
            </div>

            <!-- Duty Calculation Section -->
            <div class="duty-section mt-3" style="display: none;">
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <h6 class="mb-0 text-primary">Duty Calculation</h6>
                    <small class="text-muted">Editable rates per item</small>
                </div>
                <div class="table-responsive">
                    <table class="table table-sm table-bordered duty-table">
                        <thead class="table-light">
                            <tr class="duty-headers text-center">
                                <!-- Headers populated dynamically -->
                            </tr>
                        </thead>
                        <tbody>
                            <tr class="duty-rates text-center">
                                <!-- Editable rates will go here -->
                            </tr>
                            <tr class="duty-values text-center">
                                <!-- Calculated amounts will go here -->
                            </tr>
                        </tbody>
                        <tfoot class="table-secondary">                                    
                                <tr>
                                    <th colspan="100%" class="text-center">
                                        Total Duties: <span class="total-duty-amount">0</span>
                                    </th>
                                </tr>                                    
                        </tfoot>
                    </table>
                </div>
            </div>

            <div class="d-flex justify-content-between align-items-center mt-2">
                <button type="button" class="btn btn-sm btn-outline-info toggle-duties">Show Duties</button>
                <button type="button" class="btn btn-sm btn-danger remove-item">Remove Item</button>
            </div>
        </div>
    `;
    $('#itemsContainer').append(itemRow);

    // Populate the dropdown for the new row
    populateItemDropdown($('#itemsContainer .item-row').last());

    // Add toggle functionality
    const newRow = $('#itemsContainer .item-row').last();
    newRow.find('.toggle-duties').click(function () {
        $(this).closest('.item-row').find('.duty-section').slideToggle();
        $(this).text($(this).text() === 'Show Duties' ? 'Hide Duties' : 'Show Duties');
    });
}

function populateItemDropdown(rowElement) {
    const select = rowElement.find('.item-select');
    select.empty();
    select.append('<option value="">-- Select Item --</option>');

    if (window.itemsWithDuties && window.itemsWithDuties.length > 0) {
        window.itemsWithDuties.forEach(item => {
            select.append(`<option value="${item.id}">${item.itemName}</option>`);
        });
    }
}

function calculateItemValues(inputElement, itemIndex) {
    const itemRow = $(inputElement).closest('.item-row');
    const exchangeRate = parseFloat($('[name="PendingBL.BL.ExchangeRate"]').val()) || 0;

    // Get input values
    const quantity = parseFloat(itemRow.find('.quantity-input').val()) || 0;
    const unitValue = parseFloat(itemRow.find('.unit-value-input').val()) || 0;
    const insuranceUSD = parseFloat(itemRow.find('.insurance-usd').val()) || 0;

    // Calculate values based on your formulas
    const importValueUSD = quantity * unitValue;
    const importValuePKR = Math.round(importValueUSD * exchangeRate);
    const insurancePKR = Math.round(insuranceUSD * exchangeRate);

    // Calculate 1% Landing Charges
    const landingCharges = (importValuePKR + insurancePKR) * 0.01;

    // Calculate Assessable Value (Import Value PKR + Insurance PKR + Freight Charges + Landing Charges)
    const assessableValue = importValuePKR + insurancePKR + landingCharges;

    // Update the readonly fields
    itemRow.find('.import-value-usd').val(importValueUSD.toFixed(2));
    itemRow.find('.import-value-pkr').val(importValuePKR.toFixed(0));
    itemRow.find('.insurance-pkr').val(insurancePKR.toFixed(0));
    itemRow.find('.landing-charges').val(landingCharges.toFixed(0));
    itemRow.find('.assessable-value').val(assessableValue.toFixed(0));

    // Update duty calculations if duties are loaded
    updateDutyCalculations(itemRow, assessableValue);
}

function updateDutyCalculations(itemRow, assessableValue) {
    const dutyValuesRow = itemRow.find('.duty-values');
    const quantity = parseFloat(itemRow.find('.quantity-input').val()) || 0;
    let previousDutiesTotal = 0;

    // Loop through each duty
    dutyValuesRow.find('td.calculated-amount').each(function (index) {
        const rateInput = itemRow.find(`.duty-rate-input[data-duty-index="${index}"]`);
        const rate = parseFloat(rateInput.val()) || 0;
        const isPercentage = rateInput.data('is-percentage');

        let dutyAmount = 0;
        if (!isPercentage) {
            dutyAmount = rate * quantity;
        } else {
            dutyAmount = (assessableValue + previousDutiesTotal) * (rate / 100);
        }

        dutyAmount = Math.round(dutyAmount);
        previousDutiesTotal += dutyAmount;

        // Update displayed amount
        $(this).text(dutyAmount.toFixed(0));

        // Update hidden input fields
        const hiddenRateInput = itemRow.find(`input[name*="DutyCalculations[${index}].Rate"]`);
        const hiddenAmountInput = itemRow.find(`input[name*="DutyCalculations[${index}].Amount"]`);

        hiddenRateInput.val(rate);
        hiddenAmountInput.val(dutyAmount);
    });

    itemRow.find('.total-duty-amount').text(previousDutiesTotal.toFixed(0));
}

function loadItemDuties(selectElement, itemIndex) {
    const itemId = parseInt(selectElement.value);
    const dutySection = $(selectElement).closest('.item-row').find('.duty-section');
    const dutyTable = $(selectElement).closest('.item-row').find('.duty-table');
    const dutyHeaderRow = dutyTable.find('.duty-headers');
    const dutyRatesRow = dutyTable.find('.duty-rates');
    const dutyValuesRow = dutyTable.find('.duty-values');

    dutyHeaderRow.empty();
    dutyRatesRow.empty();
    dutyValuesRow.empty();
    dutySection.hide();

    if (!itemId) return;

    const item = window.itemsWithDuties.find(i => i.id === itemId);
    if (item && item.duties && item.duties.length > 0) {
        item.duties.forEach((duty, dutyIndex) => {
            // Header: DutyType Name
            dutyHeaderRow.append(`<th>${duty.dutyTypeName}</th>`);

            // Rates Row: Editable input field
            dutyRatesRow.append(`
                <td>
                    <input type="number" 
                           class="form-control form-control-sm text-center duty-rate-input" 
                           value="${duty.rate}" 
                           step="0.01" 
                           data-duty-index="${dutyIndex}"
                           data-is-percentage="${duty.isPercentage}"
                           data-original-rate="${duty.rate}" />
                    <div class="small text-muted mt-1">${duty.isPercentage ? '%' : 'Fixed'}</div>
                </td>
            `);

            // Values Row: Calculated amount
            dutyValuesRow.append(`
                <td class="calculated-amount text-center" data-duty-index="${dutyIndex}">0</td>
            `);

            // Hidden inputs for form submission
            dutyValuesRow.append(`
                <input type="hidden" name="Items[${itemIndex}].DutyCalculations[${dutyIndex}].DutyTypeId" value="${duty.dutyTypeId}" />
                <input type="hidden" name="Items[${itemIndex}].DutyCalculations[${dutyIndex}].Rate" value="${duty.rate}" />
                <input type="hidden" name="Items[${itemIndex}].DutyCalculations[${dutyIndex}].IsPercentage" value="${duty.isPercentage}" />
                <input type="hidden" name="Items[${itemIndex}].DutyCalculations[${dutyIndex}].Amount" value="0" />
            `);
        });

        dutySection.show();

        // Trigger initial calculation
        const itemRow = $(selectElement).closest('.item-row');
        const assessableValue = parseFloat(itemRow.find('.assessable-value').val()) || 0;
        updateDutyCalculations(itemRow, assessableValue);
    }
}

// Reset duty rates to original values
function resetDutyRates(itemRow) {
    itemRow.find('.duty-rate-input').each(function () {
        const originalRate = $(this).data('original-rate');
        $(this).val(originalRate);
    });

    const assessableValue = parseFloat(itemRow.find('.assessable-value').val()) || 0;
    updateDutyCalculations(itemRow, assessableValue);
}

// Add reset button functionality (optional)
$(document).on('click', '.reset-duties', function () {
    const itemRow = $(this).closest('.item-row');
    resetDutyRates(itemRow);
    showToast('Duty rates reset to original values', 'info');
});