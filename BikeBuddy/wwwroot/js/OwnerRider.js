function toggleActive(activeId, inactiveId) {
    const activeElement = document.getElementById(activeId);
    const inactiveElement = document.getElementById(inactiveId);
    document.getElementById(inactiveId).classList.remove("bg-orange-500", "text-white");
    document.getElementById(inactiveId).classList.add("border-orange-500", "text-orange-500");
    document.getElementById(activeId).classList.add("bg-orange-500", "text-white");
    document.getElementById(activeId).classList.remove("border-orange-500", "text-orange-500");
}
window.onload = function () {
    const params = new URLSearchParams(window.location.search);
    const serviceType = params.get('type');
    if (serviceType === 'owner') {
        toggleActive('ownerButton', 'riderButton');
    }
    else if (serviceType === 'rider') {
        toggleActive('riderButton', 'ownerButton');
    }
}
