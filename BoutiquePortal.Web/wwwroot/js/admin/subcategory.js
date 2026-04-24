(function () {
    'use strict';

    function initSubCategory() {

        // ================== FORM VALIDATION ==================
        var form = document.getElementById('SubCategoryForm');

        if (form) {
            form.addEventListener('submit', function (e) {

                var valid = true;

                // Clear previous errors
                document.querySelectorAll('[id^="err-"]').forEach(function (el) {
                    el.textContent = '';
                });

                // ================== CATEGORY DROPDOWN ==================
                var category = document.querySelector('[name="CategoryId"]');
                if (category) {
                    if (!category.value || category.value === "0") {
                        document.getElementById('err-Category').textContent = 'Please select category';
                        valid = false;
                    }
                }

                // ================== SUBCATEGORY NAME ==================
                var nameInput = document.getElementById('SubCategoryName');
                if (nameInput) {
                    var name = nameInput.value.trim();

                    if (!name) {
                        document.getElementById('err-SubCategoryName').textContent = 'SubCategory name is required';
                        valid = false;
                    } else if (name.length < 2) {
                        document.getElementById('err-SubCategoryName').textContent = 'Minimum 2 characters required';
                        valid = false;
                    }
                }

                // ================== IMAGE VALIDATION ==================
                var imageInput = document.getElementById('ImageFile');
                if (imageInput && imageInput.files.length > 0) {
                    var file = imageInput.files[0];
                    var allowed = ['image/jpeg', 'image/png', 'image/jpg'];

                    if (!allowed.includes(file.type)) {
                        document.getElementById('err-Image').textContent = 'Only JPG, JPEG, PNG allowed';
                        valid = false;
                    } else if (file.size > 2 * 1024 * 1024) {
                        document.getElementById('err-Image').textContent = 'Max file size is 2MB';
                        valid = false;
                    }
                }

                if (!valid) {
                    e.preventDefault();
                    return false;
                }
            });
        }

        // ================== IMAGE PREVIEW ==================
        var imageInput = document.getElementById('ImageFile');

        if (imageInput) {
            imageInput.addEventListener('change', function () {
                var preview = document.getElementById('imagePreview');
                if (!preview) return;

                if (this.files && this.files[0]) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        preview.src = e.target.result;
                        preview.style.display = 'block';
                    };
                    reader.readAsDataURL(this.files[0]);
                }
            });
        }

        // ================== DELETE (AJAX) ==================
        var deleteForms = document.querySelectorAll('form.delete-form');

        deleteForms.forEach(function (form) {
            form.addEventListener('submit', function (e) {
                e.preventDefault();

                if (!confirm('Are you sure you want to delete this subcategory?')) {
                    return;
                }

                var formData = new FormData(form);

                fetch(form.action, {
                    method: 'POST',
                    body: formData
                })
                    .then(function (res) {
                        if (res.ok) {
                            var row = form.closest('tr');
                            if (row) row.remove();
                        } else {
                            alert('Delete failed. Please try again.');
                        }
                    })
                    .catch(function () {
                        alert('Network error while deleting subcategory.');
                    });
            });
        });

    }

    // ================== INIT ==================
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSubCategory);
    } else {
        initSubCategory();
    }

})();