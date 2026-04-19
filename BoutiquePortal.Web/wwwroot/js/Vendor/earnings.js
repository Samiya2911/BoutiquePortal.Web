(function () {
    'use strict';

    function initEarnings() {

        // ================== ANIMATE COUNTERS ==================
        // Makes numbers count up on page load for visual effect
        var counters = document.querySelectorAll('.counter-value');

        counters.forEach(function (counter) {
            var target = parseFloat(counter.getAttribute('data-target')) || 0;
            var isRupee = counter.getAttribute('data-rupee') === 'true';
            var duration = 1500;
            var steps = 60;
            var stepVal = target / steps;
            var current = 0;
            var timer;

            timer = setInterval(function () {
                current += stepVal;

                if (current >= target) {
                    current = target;
                    clearInterval(timer);
                }

                if (isRupee) {
                    counter.textContent = '₹' + current.toLocaleString('en-IN', {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                    });
                } else {
                    counter.textContent = Math.floor(current).toLocaleString('en-IN');
                }
            }, duration / steps);
        });
    }

    // ================== INIT ==================
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initEarnings);
    } else {
        initEarnings();
    }

})();