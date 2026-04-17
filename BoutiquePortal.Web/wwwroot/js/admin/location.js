(function () {
    'use strict';

    function initLocation() {

        // ================== FORM VALIDATION ==================
        var form = document.getElementById('LocationForm');

        if (form) {
            form.addEventListener('submit', function (e) {

                var valid = true;

                // Clear errors
                document.querySelectorAll('[id^="err-"]').forEach(function (el) {
                    el.textContent = '';
                });

                // ================== COUNTRY ==================
                var country = document.getElementById('CountryId');
                if (country && !country.value) {
                    document.getElementById('err-CountryId').textContent = 'Country is required';
                    valid = false;
                }

                // ================== STATE ==================
                var state = document.getElementById('StateId');
                if (state && !state.value) {
                    document.getElementById('err-StateId').textContent = 'State is required';
                    valid = false;
                }

                // ================== CITY NAME ==================
                var cityName = document.getElementById('CityName');
                if (cityName) {
                    var name = cityName.value.trim();

                    if (!name) {
                        document.getElementById('err-CityName').textContent = 'City name is required';
                        valid = false;
                    } else if (name.length < 2) {
                        document.getElementById('err-CityName').textContent = 'Minimum 2 characters required';
                        valid = false;
                    }
                }

                // ================== CITY CODE ==================
                var cityCode = document.getElementById('CityCode');
                if (cityCode) {
                    var code = cityCode.value.trim();

                    if (!code) {
                        document.getElementById('err-CityCode').textContent = 'City code is required';
                        valid = false;
                    }
                }

                if (!valid) {
                    e.preventDefault();
                    return false;
                }
            });
        }

        // ================== COUNTRY → STATE ==================
        var countryDropdown = document.getElementById('CountryId');

        if (countryDropdown) {
            countryDropdown.addEventListener('change', function () {

                var countryId = this.value;

                var stateDropdown = document.getElementById('StateId');
                var cityDropdown = document.getElementById('CityId');

                if (stateDropdown) {
                    stateDropdown.innerHTML = '<option>Loading...</option>';
                    stateDropdown.disabled = true;
                }

                if (cityDropdown) {
                    cityDropdown.innerHTML = '<option>-- Select City --</option>';
                    cityDropdown.disabled = true;
                }

                if (!countryId) return;

                fetch('/Admin/City/GetStatesByCountry?countryId=' + countryId)
                    .then(res => res.json())
                    .then(data => {

                        var options = '<option value="">-- Select State --</option>';

                        data.forEach(function (state) {
                            options += `<option value="${state.stateId}">${state.stateName}</option>`;
                        });

                        stateDropdown.innerHTML = options;
                        stateDropdown.disabled = false;
                    })
                    .catch(() => {
                        alert('Error loading states');
                    });
            });
        }

        // ================== STATE → CITY ==================
        var stateDropdown = document.getElementById('StateId');

        if (stateDropdown) {
            stateDropdown.addEventListener('change', function () {

                var stateId = this.value;

                var cityDropdown = document.getElementById('CityId');

                if (cityDropdown) {
                    cityDropdown.innerHTML = '<option>Loading...</option>';
                    cityDropdown.disabled = true;
                }

                if (!stateId) return;

                fetch('/Admin/City/GetCitiesByState?stateId=' + stateId)
                    .then(res => res.json())
                    .then(data => {

                        var options = '<option value="">-- Select City --</option>';

                        data.forEach(function (city) {
                            options += `<option value="${city.cityId}">${city.cityName}</option>`;
                        });

                        cityDropdown.innerHTML = options;
                        cityDropdown.disabled = false;
                    })
                    .catch(() => {
                        alert('Error loading cities');
                    });
            });
        }

        // ================== DELETE (AJAX) ==================
        var deleteForms = document.querySelectorAll('form.delete-form');

        deleteForms.forEach(function (form) {
            form.addEventListener('submit', function (e) {
                e.preventDefault();

                if (!confirm('Are you sure you want to delete this record?')) {
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
                            alert('Delete failed');
                        }
                    })
                    .catch(function () {
                        alert('Network error');
                    });
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