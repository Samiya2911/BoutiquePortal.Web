(function () {
    'use strict';

    function initVendor() {

        var countryDropdown = document.getElementById('CountryId');
        var stateDropdown = document.getElementById('StateId');
        var cityDropdown = document.getElementById('CityId');

        // ================== FORM VALIDATION ==================
        var form = document.getElementById('VendorForm');

        if (form) {
            form.addEventListener('submit', function (e) {

                var valid = true;

                document.querySelectorAll('[id^="err-"]').forEach(function (el) {
                    el.textContent = '';
                });

                var vendorName = document.getElementById('VendorName');
                if (vendorName && !vendorName.value.trim()) {
                    document.getElementById('err-VendorName').textContent = 'Vendor name is required';
                    valid = false;
                }

                var brandName = document.getElementById('BrandName');
                if (brandName && !brandName.value.trim()) {
                    document.getElementById('err-BrandName').textContent = 'Brand name is required';
                    valid = false;
                }

                var email = document.getElementById('Email');
                if (email && !email.value.trim()) {
                    document.getElementById('err-Email').textContent = 'Email is required';
                    valid = false;
                }

                var password = document.getElementById('Password');
                if (password && !password.value.trim()) {
                    document.getElementById('err-Password').textContent = 'Password is required';
                    valid = false;
                }

                var phone = document.getElementById('Phone');
                if (phone && !phone.value.trim()) {
                    document.getElementById('err-Phone').textContent = 'Phone is required';
                    valid = false;
                }

                if (countryDropdown && !countryDropdown.value) {
                    document.getElementById('err-CountryId').textContent = 'Please select a country';
                    valid = false;
                }

                if (stateDropdown && !stateDropdown.value) {
                    document.getElementById('err-StateId').textContent = 'Please select a state';
                    valid = false;
                }

                if (cityDropdown && !cityDropdown.value) {
                    document.getElementById('err-CityId').textContent = 'Please select a city';
                    valid = false;
                }

                if (!valid) {
                    e.preventDefault();
                    return false;
                }
            });
        }

        // ================== HELPER: LOAD STATES ==================
        function loadStates(countryId, selectedState, callback) {

            stateDropdown.innerHTML = '<option>Loading...</option>';
            stateDropdown.disabled = true;
            cityDropdown.innerHTML = '<option value="">-- Select City --</option>';
            cityDropdown.disabled = true;

            if (!countryId) {
                stateDropdown.innerHTML = '<option value="">-- Select State --</option>';
                stateDropdown.disabled = false;
                return;
            }

            fetch('/Admin/Vendor/GetStatesByCountry?countryId=' + countryId)
                .then(res => res.json())
                .then(data => {

                    var options = '<option value="">-- Select State --</option>';

                    data.forEach(s => {
                        var sel = (s.stateId == selectedState) ? ' selected' : '';
                        options += `<option value="${s.stateId}"${sel}>${s.stateName}</option>`;
                    });

                    stateDropdown.innerHTML = options;
                    stateDropdown.disabled = false;

                    if (callback) callback();
                })
                .catch(() => {
                    stateDropdown.innerHTML = '<option value="">-- Select State --</option>';
                    stateDropdown.disabled = false;
                });
        }

        // ================== HELPER: LOAD CITIES ==================
        function loadCities(stateId, selectedCity) {

            cityDropdown.innerHTML = '<option>Loading...</option>';
            cityDropdown.disabled = true;

            if (!stateId) {
                cityDropdown.innerHTML = '<option value="">-- Select City --</option>';
                cityDropdown.disabled = false;
                return;
            }

            fetch('/Admin/Vendor/GetCitiesByState?stateId=' + stateId)
                .then(res => res.json())
                .then(data => {

                    var options = '<option value="">-- Select City --</option>';

                    data.forEach(c => {
                        var sel = (c.cityId == selectedCity) ? ' selected' : '';
                        options += `<option value="${c.cityId}"${sel}>${c.cityName}</option>`;
                    });

                    cityDropdown.innerHTML = options;
                    cityDropdown.disabled = false;
                })
                .catch(() => {
                    cityDropdown.innerHTML = '<option value="">-- Select City --</option>';
                    cityDropdown.disabled = false;
                });
        }

        // ================== COUNTRY → STATE (ON CHANGE) ==================
        if (countryDropdown) {
            countryDropdown.addEventListener('change', function () {
                loadStates(this.value, null, null);
            });
        }

        // ================== STATE → CITY (ON CHANGE) ==================
        if (stateDropdown) {
            stateDropdown.addEventListener('change', function () {
                loadCities(this.value, null);
            });
        }

        // ================== EDIT MODE: AUTO SELECT COUNTRY → STATE → CITY ==================
        if (typeof selectedCountryId !== 'undefined' && selectedCountryId) {

            if (countryDropdown) {
                countryDropdown.value = selectedCountryId;
            }

            // Load states, then load cities after states are ready
            loadStates(selectedCountryId, selectedStateId, function () {
                loadCities(selectedStateId, selectedCityId);
            });
        }

        // ================== DELETE (AJAX) ==================
        var deleteForms = document.querySelectorAll('form.delete-form');

        deleteForms.forEach(function (form) {
            form.addEventListener('submit', function (e) {
                e.preventDefault();

                if (!confirm('Are you sure you want to delete this vendor?')) return;

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
        document.addEventListener('DOMContentLoaded', initVendor);
    } else {
        initVendor();
    }

})();
