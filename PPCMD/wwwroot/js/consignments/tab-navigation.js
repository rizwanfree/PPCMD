// tab-navigation.js

function initializeTabNavigation() {
    // Next tab functionality
    $(document).on('click', '.next-tab', function () {
        const currentTab = $(this).closest('.tab-pane');
        const nextTab = currentTab.next('.tab-pane');

        if (nextTab.length) {
            // Hide current tab
            currentTab.removeClass('show active');

            // Show next tab
            nextTab.addClass('show active');

            // Update tab button states
            const currentTabButton = $('button[data-bs-target="#' + currentTab.attr('id') + '"]');
            const nextTabButton = $('button[data-bs-target="#' + nextTab.attr('id') + '"]');

            currentTabButton.removeClass('active');
            nextTabButton.addClass('active');

            // Scroll to top of the form
            $('html, body').animate({
                scrollTop: $('.card-body').offset().top - 20
            }, 300);
        }
    });

    // Previous tab functionality
    $(document).on('click', '.prev-tab', function () {
        const currentTab = $(this).closest('.tab-pane');
        const prevTab = currentTab.prev('.tab-pane');

        if (prevTab.length) {
            // Hide current tab
            currentTab.removeClass('show active');

            // Show previous tab
            prevTab.addClass('show active');

            // Update tab button states
            const currentTabButton = $('button[data-bs-target="#' + currentTab.attr('id') + '"]');
            const prevTabButton = $('button[data-bs-target="#' + prevTab.attr('id') + '"]');

            currentTabButton.removeClass('active');
            prevTabButton.addClass('active');

            // Scroll to top of the form
            $('html, body').animate({
                scrollTop: $('.card-body').offset().top - 20
            }, 300);
        }
    });

    // Bootstrap tab click handler (for clicking on tab headers)
    $('button[data-bs-toggle="tab"]').on('click', function (e) {
        e.preventDefault();
        const target = $(this).data('bs-target');

        // Hide all tabs
        $('.tab-pane').removeClass('show active');

        // Show target tab
        $(target).addClass('show active');

        // Update tab button states
        $('button[data-bs-toggle="tab"]').removeClass('active');
        $(this).addClass('active');
    });
}

// Initialize all tabs when document is ready
$(document).ready(function () {
    initializeTabNavigation();
    initializeJobShipmentTab();
    initializeItemsTab();
    initializePayorderTab();

    console.log('All tabs initialized successfully');
});