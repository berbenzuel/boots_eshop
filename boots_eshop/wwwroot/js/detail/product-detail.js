
    function setMainImage(el) {
    document.getElementById("mainImage").src = el.src;
}

    function scrollThumbs(dir) {
    document.getElementById("thumbRow")
        .scrollBy({ left: dir * 100, behavior: 'smooth' });
}

    function submitColor(colorId) {
        document.getElementById('SelectedColorId').value = colorId;
        document.getElementById('colorForm').submit();
    }