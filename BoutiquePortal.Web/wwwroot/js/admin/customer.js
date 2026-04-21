(function () {
    'use strict';

    function initCustomer() {

        // ================== SEARCH ==================
        var searchInput = document.getElementById('customerSearch');

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

        // ================== DELETE (AJAX) ==================
        var deleteForms = document.querySelectorAll('form.delete-form');

        deleteForms.forEach(function (form) {
            form.addEventListener('submit', function (e) {
                e.preventDefault();

                if (!confirm('Are you sure you want to delete this customer?'))
                    return;

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
    }

    // ================== INIT ==================
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initCustomer);
    } else {
        initCustomer();
    }

})();