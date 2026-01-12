document.addEventListener('click', function (e) {
    const productItem = e.target.closest('.product-item');
    const cartBtn = e.target.closest('.openAddCart');

    // Click on cart icon
    if (cartBtn) {
        e.stopPropagation();
        const productId = cartBtn.dataset.productId;
        showCartPopup(productId);
        return;
    }

    // Click elsewhere inside product item
    if (productItem) {
        location.href = productItem.dataset.url;
    }
});