// general-consignment-create.js
let itemIndex = 0;
let payorderIndex = 0;
let itemsWithDuties = [];

// Initialize function that can be called after data is loaded
function initializeConsignmentForm() {
    // Merge with window data if available
    if (window.itemsWithDuties) {
        itemsWithDuties = window.itemsWithDuties;
    }

    initializeDateInputs();
    initializeEventHandlers();
    addItemRow();
}

// Make it globally available
window.initializeConsignmentForm = initializeConsignmentForm;

function initializeDateInputs() {
    $('.date-input').inputmask('99/99/9999', {
        placeholder: 'DD/MM/YYYY',
        clearIncomplete: true
    });
}

function initializeEventHandlers() {
    // Item handlers
    $('#addItem').click(function () {
        addItemRow();
    });

    $(document).on('click', '.remove-item', function () {
        $(this).closest('.item-row').remove();
    });

    // Exchange rate handler
    $('[name="PendingBL.BL.LC.ExchangeRate"]').on('input', function () {
        $('.item-row').each(function () {
            const itemIndex = $(this).index();
            calculateItemValues($(this).find('.quantity-input')[0], itemIndex);
        });
    });

    // IGM lookup handler
    $('input[name="PendingBL.IGM.Number"]').on('blur', function () {
        const igmNumber = $(this).val();
        const portId = $('select[name="PendingBL.IGM.PortId"]').val();
        const currentYear = new Date().getFullYear();

        if (igmNumber && portId) {
            lookupIGM(igmNumber, portId, currentYear);
        }
    });

    // Payorder handlers
    $(document).on("click", "#addPayorder", function () {
        addPayorderRow();
    });

    $(document).on("change", ".payorder-header-select", async function () {
        await handlePayorderChange(this);
    });

    $(document).on("click", ".remove-payorder", function () {
        $(this).closest("tr").remove();
        updatePayorderOrder();
    });

    // Client contact handler
    $(document).on("change", "#clientSelect", function () {
        const clientId = $(this).val();
        if (!clientId) {
            $("#contactPerson").val("");
            return;
        }

        $.get(`/GeneralConsignment/GetClientContact?clientId=${clientId}`, function (data) {
            if (data.success) {
                $("#contactPerson").val(data.contactPerson);
            } else {
                $("#contactPerson").val("");
            }
        });
    });

    // Company detail update handlers
    $(document).on("change", "select[name='PendingBL.BL.LC.ShippingLineId'], select[name='PendingBL.BL.LC.LoloId'], select[name='PendingBL.BL.LC.TerminalId']", function () {
        updateAllCompanyDetails();
    });

    // Initialize Sortable
    if (document.querySelector("#payordersTable tbody")) {
        new Sortable(document.querySelector("#payordersTable tbody"), {
            handle: ".handle",
            animation: 150,
            onEnd: function () {
                updatePayorderOrder();
            }
        });
    }
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
                    <small class="text-muted">Based on item's duty types</small>
                </div>
                <div class="table-responsive">
                    <table class="table table-sm table-bordered duty-table">
                        <thead class="table-light">
                            <tr class="duty-headers text-center"></tr>
                        </thead>
                        <tbody>
                            <tr class="duty-values text-center"></tr>
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

    // Add toggle functionality
    const newRow = $('#itemsContainer .item-row').last();
    newRow.find('.toggle-duties').click(function () {
        $(this).closest('.item-row').find('.duty-section').slideToggle();
        $(this).text($(this).text() === 'Show Duties' ? 'Hide Duties' : 'Show Duties');
    });
}

function calculateItemValues(inputElement, itemIndex) {
    const itemRow = $(inputElement).closest('.item-row');
    const exchangeRate = parseFloat($('[name="PendingBL.BL.LC.ExchangeRate"]').val()) || 0;

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

    // Calculate Assessable Value (Import Value PKR + Insurance PKR + Landing Charges)
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

    dutyValuesRow.find('td.calculated-amount').each(function (index) {
        const rate = parseFloat($(this).siblings(`input[name*="Rate"]`).eq(index).val()) || 0;
        const isPercentage = $(this).siblings(`input[name*="IsPercentage"]`).eq(index).val() === 'true';

        let dutyAmount = 0;
        if (!isPercentage) {
            dutyAmount = rate * quantity;
        } else {
            dutyAmount = (assessableValue + previousDutiesTotal) * (rate / 100);
        }

        dutyAmount = Math.round(dutyAmount);
        previousDutiesTotal += dutyAmount;

        $(this).text(dutyAmount.toFixed(0));
    });

    itemRow.find('.total-duty-amount').text(previousDutiesTotal.toFixed(0));
}

function loadItemDuties(selectElement, itemIndex) {
    const itemId = parseInt(selectElement.value);
    const dutySection = $(selectElement).closest('.item-row').find('.duty-section');
    const dutyTable = $(selectElement).closest('.item-row').find('.duty-table');
    const dutyHeaderRow = dutyTable.find('.duty-headers');
    const dutyValuesRow = dutyTable.find('.duty-values');

    dutyHeaderRow.empty();
    dutyValuesRow.empty();
    dutySection.hide();

    if (!itemId) return;

    const item = itemsWithDuties.find(i => i.id === itemId);
    if (item && item.duties && item.duties.length > 0) {
        item.duties.forEach((duty, dutyIndex) => {
            dutyHeaderRow.append(`<th>${duty.dutyTypeName} @@ ${duty.rate}${duty.isPercentage ? '%' : 'F'}</th>`);
            dutyValuesRow.append(`
                <td class="calculated-amount text-center" data-duty-index="${dutyIndex}">0</td>
                <input type="hidden" name="Items[${itemIndex}].DutyCalculations[${dutyIndex}].DutyTypeId" value="${duty.dutyTypeId}" />
                <input type="hidden" name="Items[${itemIndex}].DutyCalculations[${dutyIndex}].Rate" value="${duty.rate}" />
                <input type="hidden" name="Items[${itemIndex}].DutyCalculations[${dutyIndex}].IsPercentage" value="${duty.isPercentage}" />
            `);
        });
        dutySection.show();
    }
}

function addPayorderRow() {
    const row = `
        <tr>
            <td class="handle text-center">
                <i class="bi bi-list"></i>
                <input type="hidden" name="Payorders[${payorderIndex}].Order" value="${payorderIndex}" class="order-field" />
            </td>
            <td>
                <select name="Payorders[${payorderIndex}].PayorderHeaderId" class="form-select form-select-sm payorder-header-select">
                    <option value="">-- Select Payorder --</option>
                </select>
            </td>
            <td>
                <input type="text" name="Payorders[${payorderIndex}].Detail" class="form-control form-control-sm payorder-detail" placeholder="Details" />
            </td>
            <td>
                <input type="number" name="Payorders[${payorderIndex}].Amount" class="form-control form-control-sm text-end" step="0.01" />
            </td>
            <td class="text-center">
                <button type="button" class="btn btn-sm btn-danger remove-payorder">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        </tr>`;
    $("#payordersTable tbody").append(row);
    payorderIndex++;
}

async function handlePayorderChange(selectElement) {
    const selectedOption = $(selectElement).find('option:selected');
    const description = selectedOption.data('description');
    const payorderName = selectedOption.text().trim().toUpperCase();
    const detailInput = $(selectElement).closest('tr').find('.payorder-detail');

    const shippingLineId = $('select[name="PendingBL.BL.LC.ShippingLineId"]').val();
    const loloId = $('select[name="PendingBL.BL.LC.LoloId"]').val();
    const terminalId = $('select[name="PendingBL.BL.LC.TerminalId"]').val();

    if (companyDetailPayorders.includes(payorderName)) {
        const companyDetail = await getCompanyDetails(payorderName, shippingLineId, loloId, terminalId);
        if (companyDetail) {
            detailInput.val(companyDetail);
            showToast(`${payorderName} detail auto-populated`, 'success');
        } else {
            if (description && description.trim() !== '') {
                detailInput.val(description);
            } else {
                detailInput.val('');
                showToast(`Please select ${getRequiredCompanyType(payorderName)} first`, 'warning');
            }
        }
    } else {
        if (description && description.trim() !== '') {
            detailInput.val(description);
        } else {
            detailInput.val('');
        }
    }
}

function updatePayorderOrder() {
    $("#payordersTable tbody tr").each(function (i) {
        $(this).find(".order-field").val(i);
        $(this).find("td:first").contents().filter(function () {
            return this.nodeType === 3;
        }).remove();
        $(this).find("td:first").prepend((i + 1) + ". ");
    });
}

function lookupIGM(igmNumber, portId, year) {
    $.get(`/GeneralConsignment/GetIGMDetails?igmNumber=${igmNumber}&portId=${portId}&year=${year}`, function (data) {
        if (data.success) {
            $('input[name="PendingBL.IGM.Date"]').val(formatDateForInput(data.igmDate));
            $('input[name="PendingBL.IGM.Vessel"]').val(data.vessel);
            showToast('IGM found', 'success');
        } else {
            $('input[name="PendingBL.IGM.Date"]').val('');
            $('input[name="PendingBL.IGM.Vessel"]').val('');
            showToast('IGM not found for the selected port and year. Please enter details manually.', 'info');
        }
    }).fail(function () {
        showToast('Error looking up IGM details.', 'error');
    });
}

function formatDateForInput(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
}

function showToast(message, type = 'info') {
    $('.toast').remove();
    const toast = $(`
        <div class="toast align-items-center text-white bg-${type === 'success' ? 'success' : type === 'error' ? 'danger' : 'info'} border-0 position-fixed top-0 end-0 m-3" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `);
    $('body').append(toast);
    const bsToast = new bootstrap.Toast(toast[0]);
    bsToast.show();
}

const companyDetailPayorders = [
    'CONTAINER DEPOSIT',
    'CONTAINER RENT',
    'DELIVERY ORDER',
    'LIFT ON LIFT OFF',
    'TERMINAL WHARFAGE'
];

function getCompanyDetails(payorderName, shippingLineId, loloId, terminalId) {
    return new Promise((resolve) => {
        let endpoint = '';
        let id = 0;

        if (payorderName === 'CONTAINER DEPOSIT' || payorderName === 'CONTAINER RENT' || payorderName === 'DELIVERY ORDER') {
            endpoint = '/GeneralConsignment/GetShippingLineDetails';
            id = shippingLineId;
        } else if (payorderName === 'LIFT ON LIFT OFF') {
            endpoint = '/GeneralConsignment/GetLoloDetails';
            id = loloId;
        } else if (payorderName === 'TERMINAL WHARFAGE') {
            endpoint = '/GeneralConsignment/GetTerminalDetails';
            id = terminalId;
        }

        if (endpoint && id) {
            $.get(`${endpoint}?id=${id}`, function (data) {
                if (data.success) {
                    if (data.ntn && data.ntn.trim() !== '' && data.ntn !== 'N/A') {
                        resolve(`${data.name} (NTN: ${data.ntn})`);
                    } else {
                        resolve(data.name);
                    }
                } else {
                    resolve('');
                }
            }).fail(() => resolve(''));
        } else {
            resolve('');
        }
    });
}

function getRequiredCompanyType(payorderName) {
    switch (payorderName) {
        case 'CONTAINER DEPOSIT':
        case 'CONTAINER RENT':
        case 'DELIVERY ORDER':
            return 'Shipping Line';
        case 'LIFT ON LIFT OFF':
            return 'LOLO';
        case 'TERMINAL WHARFAGE':
            return 'Terminal';
        default:
            return 'Company';
    }
}

function updateAllCompanyDetails() {
    $('.payorder-header-select').each(async function () {
        const selectedOption = $(this).find('option:selected');
        const payorderName = selectedOption.text().trim().toUpperCase();

        if (companyDetailPayorders.includes(payorderName)) {
            const detailInput = $(this).closest('tr').find('.payorder-detail');
            const shippingLineId = $('select[name="PendingBL.BL.LC.ShippingLineId"]').val();
            const loloId = $('select[name="PendingBL.BL.LC.LoloId"]').val();
            const terminalId = $('select[name="PendingBL.BL.LC.TerminalId"]').val();

            const companyDetail = await getCompanyDetails(payorderName, shippingLineId, loloId, terminalId);
            if (companyDetail) {
                detailInput.val(companyDetail);
            }
        }
    });
}