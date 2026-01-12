window.productId = null;

async function showCartPopup(productId , color = null) {
    // Store globally
    window.productId = productId;

    let url = `/Cart/Add?productId=${productId}`;
    if (color) url += `&colorId=${color}`;

    const response = await fetch(url);
    const html = await response.text();

    // Remove old popup
    const old = document.querySelector('#componentHost');
    if (old) old.remove();

    // Insert new popup
    const container = document.createElement('div');
    container.id = 'componentHost';
    container.innerHTML = html;
    document.body.appendChild(container);
    document.body.classList.add('modal-open');
}

document.body.addEventListener('change', async function(e) {
    if (e.target.name === 'SelectedColorId') {
        const selectedColor = e.target.value;

        const response = await fetch(`/Cart/Add?productId=${window.productId}&colorId=${selectedColor}`);
        const html = await response.text();

        const container = document.querySelector('#componentHost');
        container.innerHTML = html;
    }
});


function closeAddCart() {
    document.getElementById('componentHost')?.remove();
    document.body.classList.remove('modal-open');
}

// document.body.addEventListener('submit', async function (e) {
//     if (e.target.id !== 'addCartForm') return;
//
//     e.preventDefault();
//
//     const form = e.target;
//     const formData = new FormData(form);
//
//     const response = await fetch(form.action, {
//         method: 'POST',
//         body: formData,
//         credentials: 'same-origin'
//     });
// });
