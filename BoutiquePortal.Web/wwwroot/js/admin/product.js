(function () {
    'use strict';

    function initProduct() {

        var categoryDropdown = document.getElementById('CategoryId');
        var subCategoryDropdown = document.getElementById('SubCategoryId');
        var vendorDropdown = document.getElementById('VendorId');
        var brandInput = document.getElementById('BrandName');

        // ================== FORM VALIDATION ==================
        var form = document.getElementById('ProductForm');

        if (form) {
            form.addEventListener('submit', function (e) {

                var valid = true;

                document.querySelectorAll('[id^="err-"]').forEach(function (el) {
                    el.textContent = '';
                });

                var productName = document.getElementById('ProductName');
                if (productName && !productName.value.trim()) {
                    document.getElementById('err-ProductName').textContent = 'Product name is required';
                    valid = false;
                }

                var price = document.getElementById('Price');
                if (price && (!price.value || parseFloat(price.value) <= 0)) {
                    document.getElementById('err-Price').textContent = 'Price must be greater than 0';
                    valid = false;
                }

                if (categoryDropdown && !categoryDropdown.value) {
                    document.getElementById('err-CategoryId').textContent = 'Please select a category';
                    valid = false;
                }

                if (subCategoryDropdown && !subCategoryDropdown.value) {
                    document.getElementById('err-SubCategoryId').textContent = 'Please select a sub category';
                    valid = false;
                }

                if (vendorDropdown && !vendorDropdown.value) {
                    document.getElementById('err-VendorId').textContent = 'Please select a vendor';
                    valid = false;
                }

                if (!valid) {
                    e.preventDefault();
                    return false;
                }
            });
        }

        // ================== CATEGORY → SUB CATEGORY (ON CHANGE) ==================
        if (categoryDropdown) {
            categoryDropdown.addEventListener('change', function () {

                var categoryId = this.value;

                subCategoryDropdown.innerHTML = '<option value="">Loading...</option>';
                subCategoryDropdown.disabled = true;

                if (!categoryId) {
                    subCategoryDropdown.innerHTML = '<option value="">-- Select Sub Category --</option>';
                    subCategoryDropdown.disabled = false;
                    return;
                }

                fetch('/Admin/Product/GetSubCategoriesByCategory?categoryId=' + categoryId)
                    .then(res => res.json())
                    .then(data => {

                        var options = '<option value="">-- Select Sub Category --</option>';

                        data.forEach(sc => {
                            // ✅ FIX: Use PascalCase — ASP.NET Core Json() returns PascalCase by default
                            var id = sc.SubCategoryId || sc.subCategoryId;
                            var name = sc.SubCategoryName || sc.subCategoryName;
                            options += `<option value="${id}">${name}</option>`;
                        });

                        subCategoryDropdown.innerHTML = options;
                        subCategoryDropdown.disabled = false;
                    })
                    .catch(() => {
                        subCategoryDropdown.innerHTML = '<option value="">-- Select Sub Category --</option>';
                        subCategoryDropdown.disabled = false;
                    });
            });
        }

        // ================== VENDOR → BRAND NAME (ON CHANGE) ==================
        if (vendorDropdown) {
            vendorDropdown.addEventListener('change', function () {

                var vendorId = this.value;

                if (!vendorId) {
                    brandInput.value = '';
                    return;
                }

                fetch('/Admin/Product/GetVendorBrand?vendorId=' + vendorId)
                    .then(res => res.json())
                    .then(data => {
                        // ✅ Handle both camelCase and PascalCase
                        brandInput.value = data.brandName || data.BrandName || '';
                    })
                    .catch(() => {
                        brandInput.value = '';
                    });
            });
        }

        // ================== EDIT MODE: AUTO SELECT CATEGORY + SUB CATEGORY ==================
        if (typeof selectedCategoryId !== 'undefined' && selectedCategoryId) {

            if (categoryDropdown) {
                categoryDropdown.value = selectedCategoryId;
            }

            fetch('/Admin/Product/GetSubCategoriesByCategory?categoryId=' + selectedCategoryId)
                .then(res => res.json())
                .then(data => {

                    var options = '<option value="">-- Select Sub Category --</option>';

                    data.forEach(sc => {
                        // ✅ FIX: Handle both PascalCase and camelCase
                        var id = sc.SubCategoryId || sc.subCategoryId;
                        var name = sc.SubCategoryName || sc.subCategoryName;

                        if (id == selectedSubCategoryId) {
                            options += `<option value="${id}" selected>${name}</option>`;
                        } else {
                            options += `<option value="${id}">${name}</option>`;
                        }
                    });

                    subCategoryDropdown.innerHTML = options;
                    subCategoryDropdown.disabled = false;
                });
        }

        // ================== EDIT MODE: AUTO FILL BRAND NAME ==================
        if (typeof selectedVendorId !== 'undefined' && selectedVendorId) {

            if (vendorDropdown) {
                vendorDropdown.value = selectedVendorId;
            }

            if (brandInput && !brandInput.value) {
                fetch('/Admin/Product/GetVendorBrand?vendorId=' + selectedVendorId)
                    .then(res => res.json())
                    .then(data => {
                        brandInput.value = data.brandName || data.BrandName || '';
                    });
            }
        }

        // ================== DELETE (AJAX) ==================
        var deleteForms = document.querySelectorAll('form.delete-form');

        deleteForms.forEach(function (form) {
            form.addEventListener('submit', function (e) {
                e.preventDefault();

                if (!confirm('Are you sure you want to delete this product?')) return;

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
                            alert('Delete failed');
                        }
                    })
                    .catch(() => alert('Network error'));
            });
        });
    }

    // ================== INIT ==================
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initProduct);
    } else {
        initProduct();
    }

})();
