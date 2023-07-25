const items = document.querySelectorAll('.item');
const contents = document.querySelectorAll('.hidden-visible-box');

function removeActive() {
    items.forEach(item => {
        item.classList.remove('active-item');
    });
    contents.forEach(item => {
        item.classList.remove('active');
    });
}

items.forEach((item, index) => {
    item.addEventListener('click', function () {
        removeActive();
        item.classList.add('active-item');
        contents[index].classList.add('active');
    })
})