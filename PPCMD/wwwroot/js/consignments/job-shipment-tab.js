// job-shipment-tab.js

function initializeJobShipmentTab() {
    initializeDateInputs();
    initializeClientLookup();
    initializeIGMLookup();
    initializeShippingLineLookup();
    initializeLoloLookup();
    initializeTerminalLookup();
}

function initializeDateInputs() {
    $('.date-input').inputmask('99/99/9999', {
        placeholder: 'DD/MM/YYYY',
        clearIncomplete: true
    });
}

function initializeClientLookup() {
    $(document).on("change", "#clientSelect", function () {
        const clientId = $(this).val();
        if (!clientId) {
            $("#contactPerson").val("");
            return;
        }

        $.get(`/HC/GetClientContact?clientId=${clientId}`, function (data) {
            if (data.success) {
                $("#contactPerson").val(data.contactPerson);
            } else {
                $("#contactPerson").val("");
            }
        });
    });
}

function initializeIGMLookup() {
    // Manual lookup button
    $(document).on("click", "#lookupIGM", function () {
        performIGMLookup();
    });

    // Auto lookup on blur (optional)
    $(document).on("blur", 'input[name="PendingBL.IGM.Number"]', function () {
        // Small delay to allow button click to register first
        setTimeout(() => {
            if (!$(this).data('lookup-performed')) {
                performIGMLookup();
            }
        }, 100);
    });
}

function performIGMLookup() {
    const igmNumber = $('input[name="PendingBL.IGM.Number"]').val();
    const portId = $('select[name="PendingBL.IGM.PortId"]').val();

    console.log(igmNumber);
    console.log(portId);

    if (!igmNumber) {
        showToast('Please enter IGM number', 'warning');
        return;
    }

    if (!portId) {
        showToast('Please select a port first', 'warning');
        return;
    }

    const currentYear = new Date().getFullYear();

    showToast('Looking up IGM details...', 'info');

    $.get(`/HC/GetIGMDetails?igmNumber=${igmNumber}&portId=${portId}&year=${currentYear}`, function (data) {
        if (data.success) {
            // Set the lookup performed flag to prevent double execution
            $('input[name="PendingBL.IGM.Number"]').data('lookup-performed', true);

            // Populate the fields
            if (data.igmDate) {
                $('input[name="PendingBL.IGM.Date"]').val(formatDateForInput(data.igmDate));
            }
            if (data.vessel) {
                $('input[name="PendingBL.IGM.Vessel"]').val(data.vessel);
            }

            showToast('IGM details found and populated', 'success');

            // Clear the flag after a short delay
            setTimeout(() => {
                $('input[name="PendingBL.IGM.Number"]').data('lookup-performed', false);
            }, 1000);

        } else {
            showToast(data.message || 'IGM not found for the selected port and year', 'info');
            // Clear fields if not found
            $('input[name="PendingBL.IGM.Date"]').val('');
            $('input[name="PendingBL.IGM.Vessel"]').val('');
        }
    }).fail(function (xhr, status, error) {
        console.error('IGM lookup failed:', error);
        showToast('Error looking up IGM details: ' + error, 'error');
    });
}

function formatDateForInput(dateString) {
    if (!dateString) return '';

    try {
        const date = new Date(dateString);
        if (isNaN(date.getTime())) {
            return dateString; // Return original if invalid date
        }

        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();

        return `${day}/${month}/${year}`;
    } catch (e) {
        console.error('Date formatting error:', e);
        return dateString; // Return original string if formatting fails
    }
}

function showToast(message, type = 'info') {
    $('.toast').remove();

    const bgClass = type === 'success' ? 'bg-success' :
        type === 'error' ? 'bg-danger' :
            type === 'warning' ? 'bg-warning' : 'bg-info';

    const toast = $(`
        <div class="toast align-items-center text-white ${bgClass} border-0 position-fixed top-0 end-0 m-3" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `);

    $('body').append(toast);
    const bsToast = new bootstrap.Toast(toast[0]);
    bsToast.show();
}


function initializeShippingLineLookup() {
    $(document).on("change", "#shippingLineSelect", function () {
        const shippingLineId = $(this).val();
        if (!shippingLineId) {
            clearShippingLineInfo();
            return;
        }

        $.get(`/HC/GetShippingLineDetails?id=${shippingLineId}`, function (data) {
            if (data.success) {
                // Display shipping line info
                $('#shippingLineName').text(data.name);
                $('#shippingLineNTN').text(data.ntn || '-');
                $('#shippingLinePhone').text(data.phone || '-');

                // Auto-populate payorders for shipping line
                autoPopulateShippingLinePayorders(data.name, data.ntn);
            } else {
                clearShippingLineInfo();
            }
        });
    });
}


function initializeLoloLookup() {
    $(document).on("change", "#loloSelect", function () {
        const loloId = $(this).val();
        if (!loloId) return;

        $.get(`/HC/GetLoloDetails?id=${loloId}`, function (data) {
            if (data.success) {
                autoPopulateLoloPayorders(data.name, data.ntn);
            }
        });
    });
}

function initializeTerminalLookup() {
    $(document).on("change", "#terminalSelect", function () {
        const terminalId = $(this).val();
        if (!terminalId) return;

        $.get(`/HC/GetTerminalDetails?id=${terminalId}`, function (data) {
            if (data.success) {
                autoPopulateTerminalPayorders(data.name, data.ntn);
            }
        });
    });
}

function clearShippingLineInfo() {
    $('#shippingLineName').text('-');
    $('#shippingLineNTN').text('-');
    $('#shippingLinePhone').text('-');
}

function autoPopulateShippingLinePayorders(companyName, ntn) {
    const payorderDetails = generateCompanyDetail(companyName, ntn);

    // Define shipping line specific payorders
    const shippingLinePayorders = [
        { name: 'CONTAINER RENT', detail: payorderDetails },
        { name: 'CONTAINER DEPOSIT', detail: payorderDetails },
        { name: 'DELIVERY ORDER', detail: payorderDetails },
        { name: 'ENDORSEMENT', detail: payorderDetails }
    ];

    populatePayorders(shippingLinePayorders);
}

function autoPopulateLoloPayorders(companyName, ntn) {
    const payorderDetails = generateCompanyDetail(companyName, ntn);

    // Lolo specific payorder
    const loloPayorders = [
        { name: 'LIFT ON LIFT OFF', detail: payorderDetails }
    ];

    populatePayorders(loloPayorders);
}

function autoPopulateTerminalPayorders(companyName, ntn) {
    const payorderDetails = generateCompanyDetail(companyName, ntn);

    // Terminal specific payorder
    const terminalPayorders = [
        { name: 'TERMINAL WHARFAGE', detail: payorderDetails }
    ];

    populatePayorders(terminalPayorders);
}

function generateCompanyDetail(companyName, ntn) {
    if (ntn) {
        return `${companyName} (NTN:${ntn})`;
    }
    return companyName;
}

function populatePayorders(payordersToPopulate) {
    // Wait a bit for payorders to be initialized
    setTimeout(() => {
        payordersToPopulate.forEach(payorder => {
            // Find the row with matching payorder name
            const rows = $('#payordersTable tbody tr');
            let found = false;

            rows.each(function () {
                const row = $(this);
                const nameCell = row.find('td:nth-child(2)'); // Second column has the name
                const nameText = nameCell.text().trim();

                if (nameText === payorder.name) {
                    // Found matching payorder, update the detail
                    const detailInput = row.find('.payorder-detail');
                    detailInput.val(payorder.detail);
                    found = true;
                    return false; // Break the loop
                }
            });

            if (!found) {
                console.log(`Payorder "${payorder.name}" not found in the table`);
            }
        });

        showToast('Payorder details updated automatically', 'success');
    }, 500);
}