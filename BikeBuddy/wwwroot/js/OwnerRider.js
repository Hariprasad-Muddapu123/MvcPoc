(function () {
    function toggleActive(activeId, inactiveId) {
        const activeElement = document.getElementById(activeId);
        const inactiveElement = document.getElementById(inactiveId);

        if (activeElement && inactiveElement) {
            inactiveElement.classList.remove("bg-orange-500", "text-white");
            inactiveElement.classList.add("border-orange-500", "text-orange-500");

            activeElement.classList.add("bg-orange-500", "text-white");
            activeElement.classList.remove("border-orange-500", "text-orange-500");
        }
    }

    window.onload = function () {
        const params = new URLSearchParams(window.location.search);
        const serviceType = params.get('type');

        if (serviceType === 'owner') {
            toggleActive('ownerButton', 'riderButton');
        } else if (serviceType === 'rider') {
            toggleActive('riderButton', 'ownerButton');
        }
    };
})();
