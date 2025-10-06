// payorder-tab.js

let payorderIndex = 0;

function initializePayorderTab() {
    populateAllPayorders();

    // Calculate total when amounts change
    $(document).on('input', '.payorder-amount', function () {
        calculateTotalAmount();
    });

    // Initialize total calculation
    calculateTotalAmount();
}

function populateAllPayorders() {
    const tbody = $('#payordersTable tbody');
    tbody.empty();

    if (!window.payorderHeaders || window.payorderHeaders.length === 0) {
        tbody.append(`
            <tr>
                <td colspan="4" class="text-center text-muted py-3">
                    No payorder headers found. Please configure payorder headers in the system.
                </td>
            </tr>
        `);
        return;
    }

    window.payorderHeaders.forEach((header, index) => {
        const rowIndex = payorderIndex++;
        const row = `
            <tr>
                <td class="text-center handle">
                    <i class="bi bi-list"></i>
                    <input type="hidden" name="Payorders[${rowIndex}].Order" value="${index}" class="order-field" />
                    <input type="hidden" name="Payorders[${rowIndex}].PayorderHeaderId" value="${header.id}" />
                </td>
                <td>
                    <strong>${header.name}</strong>
                </td>
                <td>
                    <input type="text" 
                           name="Payorders[${rowIndex}].Detail" 
                           class="form-control form-control-sm payorder-detail" 
                           value="${header.description || ''}" 
                           placeholder="Enter details..." />
                </td>
                <td>
                    <input type="number" 
                           name="Payorders[${rowIndex}].Amount" 
                           class="form-control form-control-sm text-end payorder-amount" 
                           step="0.01" 
                           min="0"
                           value="0.00"
                           placeholder="0.00" />
                </td>
            </tr>
        `;
        tbody.append(row);
    });

    // Initialize Sortable for reordering
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

function calculateTotalAmount() {
    let total = 0;

    $('.payorder-amount').each(function () {
        const amount = parseFloat($(this).val()) || 0;
        total += amount;
    });

    $('#totalPayorderAmount').text(total.toFixed(2));
}

function updatePayorderOrder() {
    $("#payordersTable tbody tr").each(function (index) {
        // Update hidden order field
        $(this).find(".order-field").val(index);

        // Update visible order number
        $(this).find("td:first").contents().filter(function () {
            return this.nodeType === 3;
        }).remove();

        $(this).find("td:first").prepend((index + 1) + ". ");
    });
}

// Optional: Clear all amounts
function clearAllAmounts() {
    $('.payorder-amount').val('0.00').trigger('input');
    showToast('All amounts cleared', 'info');
}

// Optional: Add quick action buttons to header if needed
function addQuickActions() {
    const quickActions = `
        <div class="mb-2 text-end">
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="clearAllAmounts()">
                <i class="bi bi-arrow-clockwise"></i> Clear All Amounts
            </button>
        </div>
    `;
    $('#payordersTable').before(quickActions);
}