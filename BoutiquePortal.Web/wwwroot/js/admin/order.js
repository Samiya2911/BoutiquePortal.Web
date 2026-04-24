(function () {
    'use strict';

    function initOrder() {

        // ================== DELETE (AJAX) ==================
        var deleteForms = document.querySelectorAll('form.delete-form');

        deleteForms.forEach(function (form) {
            form.addEventListener('submit', function (e) {
                e.preventDefault();

                if (!confirm('Are you sure you want to delete this order? This cannot be undone.')) return;

                var formData = new FormData(form);

                fetch(form.action, {
                    method: 'POST',
                    body: formData
                })
                    .then(res => {
                        if (res.ok) {
                            var row = form.closest('tr');
                            if (row) row.remove();
                        } else {
                            alert('Delete failed. Please try again.');
                        }
                    })
                    .catch(() => alert('Network error. Please try again.'));
            });
        });

        // ================== STATUS BADGE COLOR (live update) ==================
        // After status form submit — update badge color without page reload
        var statusForm = document.getElementById('StatusUpdateForm');

        if (statusForm) {
            statusForm.addEventListener('submit', function (e) {
                
            });
        }

        // ================== ORDER ROW CLICK → DETAILS ==================
        var orderRows = document.querySelectorAll('tr.clickable-row');

        orderRows.forEach(function (row) {
            row.addEventListener('click', function () {
                var url = this.getAttribute('data-href');
                if (url) window.location.href = url;
            });
            row.style.cursor = 'pointer';
        });

        // ================== SEARCH / FILTER ==================
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

        // ================== STATUS FILTER DROPDOWN ==================
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
    }

    // ================== INIT ==================
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initOrder);
    } else {
        initOrder();
    }

})();