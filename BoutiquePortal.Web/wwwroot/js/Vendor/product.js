(function () {
    'use strict';

    function initProduct() {

        var categoryDropdown = document.getElementById('CategoryId');
        var subCategoryDropdown = document.getElementById('SubCategoryId');

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

                if (!valid) {
                    e.preventDefault();
                    return false;
                }
            });
        }

        // ================== IMAGE PREVIEW ==================
        var imageFileInput = document.getElementById('ImageFile');
        var imagePreview = document.getElementById('imagePreview');

        if (imageFileInput) {
            imageFileInput.addEventListener('change', function () {
                var file = this.files[0];
                if (file && imagePreview) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        imagePreview.src = e.target.result;
                        imagePreview.style.display = 'block';
                    };
                    reader.readAsDataURL(file);
                }
            });
        }

        // ================== CATEGORY → SUB CATEGORY ==================
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

                // ✅ Vendor area AJAX endpoint
                fetch('/Vendor/Product/GetSubCategoriesByCategory?categoryId=' + categoryId)
                    .then(res => res.json())
                    .then(data => {

                        var options = '<option value="">-- Select Sub Category --</option>';
                        data.forEach(sc => {
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

        // ================== EDIT MODE: AUTO SELECT CATEGORY + SUB CATEGORY ==================
        if (typeof selectedCategoryId !== 'undefined' && selectedCategoryId) {

            if (categoryDropdown) {
                categoryDropdown.value = selectedCategoryId;
            }

            fetch('/Vendor/Product/GetSubCategoriesByCategory?categoryId=' + selectedCategoryId)
                .then(res => res.json())
                .then(data => {

                    var options = '<option value="">-- Select Sub Category --</option>';
                    data.forEach(sc => {
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