
    function setMainImage(el) {
    document.getElementById("mainImage").src = el.src;
}

    function scrollThumbs(dir) {
    document.getElementById("thumbRow")
        .scrollBy({ left: dir * 100, behavior: 'smooth' });
}

    function submitColor(productId, colorId) {
        location.href = '/ProductDetail?productId=' + productId + '&colorId=' + colorId;
    }
    