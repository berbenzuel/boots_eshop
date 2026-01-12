document.querySelectorAll('form[class="quantity-form"]').forEach((el) => {
    console.log(el.querySelector('input[name="Quantity"]'));
    el.querySelector('input[name="Quantity"]').addEventListener('change', () => {
        console.log(el)
        el.submit();
    })
})


