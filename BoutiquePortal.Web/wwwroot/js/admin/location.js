(function () {
    'use strict';

    function initLocation() {

        // ================== FORM VALIDATION ==================
        var form = document.getElementById('LocationForm');

        if (form) {
            form.addEventListener('submit', function (e) {

                var valid = true;

                // Clear old errors
                document.querySelectorAll('[id^="err-"]').forEach(function (el) {
                    el.textContent = '';
                });

                // COUNTRY VALIDATION
                var country = document.getElementById('CountryId');
                if (country && !country.value) {
                    document.getElementById('err-CountryId').textContent = 'Country is required';
                    valid = false;
                }

                // STATE VALIDATION
                var state = document.getElementById('StateId');
                if (state && !state.value) {
                    document.getElementById('err-StateId').textContent = 'State is required';
                    valid = false;
                }

                // CITY NAME VALIDATION
                var cityName = document.getElementById('CityName');
                if (cityName && !cityName.value.trim()) {
                    document.getElementById('err-CityName').textContent = 'City name is required';
                    valid = false;
                }

                // CITY CODE VALIDATION
                var cityCode = document.getElementById('CityCode');
                if (cityCode && !cityCode.value.trim()) {
                    document.getElementById('err-CityCode').textContent = 'City code is required';
                    valid = false;
                }

                if (!valid) {
                    e.preventDefault();
                    return false;
                }
            });
        }

        var countryDropdown = document.getElementById('CountryId');
        var stateDropdown = document.getElementById('StateId');

        // ================== COUNTRY → STATE ==================
        if (countryDropdown) {
            countryDropdown.addEventListener('change', function () {

                var countryId = this.value;

                stateDropdown.innerHTML = '<option>Loading...</option>';
                stateDropdown.disabled = true;

                if (!countryId) {
                    stateDropdown.innerHTML = '<option value="">-- Select State --</option>';
                    stateDropdown.disabled = false;
                    return;
                }

                fetch('/Admin/City/GetStatesByCountry?countryId=' + countryId)
                    .then(res => res.json())
                    .then(data => {

                        var options = '<option value="">-- Select State --</option>';

                        data.forEach(state => {
                            options += `<option value="${state.stateId}">${state.stateName}</option>`;
                        });

                        stateDropdown.innerHTML = options;
                        stateDropdown.disabled = false;
                    });
            });
        }

       
        if (typeof selectedCountryId !== 'undefined' && selectedCountryId) {

            if (countryDropdown) {
                countryDropdown.value = selectedCountryId;
            }

            fetch('/Admin/City/GetStatesByCountry?countryId=' + selectedCountryId)
                .then(res => res.json())
                .then(data => {

                    var options = '<option value="">-- Select State --</option>';

                    data.forEach(state => {

                        // ✅ AUTO SELECT STATE
                        if (state.stateId == selectedStateId) {
                            options += `<option value="${state.stateId}" selected>${state.stateName}</option>`;
                        } else {
                            options += `<option value="${state.stateId}">${state.stateName}</option>`;
                        }
                    });

                    stateDropdown.innerHTML = options;
                    stateDropdown.disabled = false;
                });
        }


        // ================== DELETE (AJAX) ==================
        var deleteForms = document.querySelectorAll('form.delete-form');

        deleteForms.forEach(function (form) {
            form.addEventListener('submit', function (e) {
                e.preventDefault();

                if (!confirm('Are you sure you want to delete this record?')) return;

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
        document.addEventListener('DOMContentLoaded', initLocation);
    } else {
        initLocation();
    }

})();