(function () {
    'use strict';

    function initVendorOrder() {

        // ================== SEARCH ==================
        var searchInput = document.getElementById('orderSearch');

        if (searchInput) {
            searchInput.addEventListener('keyup', function () {
                var searchVal = this.value.toLowerCase();
                var rows = document.querySelectorAll('tbody tr');

                rows.forEach(function (row) {
                    var text = row.textContent.toLowerCase();
                    row.style.display = text.includes(searchVal) ? '' : 'none';
                });
            });
        }

        // ================== STATUS FILTER ==================
        var statusFilter = document.getElementById('statusFilter');

        if (statusFilter) {
            statusFilter.addEventListener('change', function () {
                var selected = this.value.toLowerCase();
                var rows = document.querySelectorAll('tbody tr');

                rows.forEach(function (row) {
                    if (!selected) {
                        row.style.display = '';
                        return;
                    }
                    var statusBadge = row.querySelector('.order-status-badge');
                    if (statusBadge) {
                        var status = statusBadge.textContent.trim().toLowerCase();
                        row.style.display = status.includes(selected) ? '' : 'none';
                    }
                });
            });
        }

        // ================== DATE FILTER ==================
        var dateFilter = document.getElementById('dateFilter');

        if (dateFilter) {
            dateFilter.addEventListener('change', function () {
                var selected = this.value;
                var rows = document.querySelectorAll('tbody tr');
                var today = new Date();

                rows.forEach(function (row) {
                    if (!selected) {
                        row.style.display = '';
                        return;
                    }

                    var dateCell = row.querySelector('.order-date-cell');
                    if (!dateCell) return;

                    var rowDate = new Date(dateCell.getAttribute('data-date'));
                    var show = false;

                    if (selected === 'today') {
                        show = rowDate.toDateString() === today.toDateString();
                    } else if (selected === 'week') {
                        var weekAgo = new Date();
                        weekAgo.setDate(today.getDate() - 7);
                        show = rowDate >= weekAgo;
                    } else if (selected === 'month') {
                        show = rowDate.getMonth() === today.getMonth() &&
                            rowDate.getFullYear() === today.getFullYear();
                    }

                    row.style.display = show ? '' : 'none';
                });
            });
        }
    }

    // ================== INIT ==================
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initVendorOrder);
    } else {
        initVendorOrder();
    }

})();