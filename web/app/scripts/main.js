const defaultSort = 99;

let clickPnpItem = (event) => {

    event.preventDefault();
    event.stopPropagation();

    let curElem = event.currentTarget;

    var items = document.querySelectorAll('.pnp-item');

    items.forEach(element => {

        console.log(element !== curElem);
        if (element !== curElem) {
            element.classList.remove('show');
        }

    });

    if (curElem.classList.contains('show')) {
        curElem.classList.remove('show');
    } else {
        curElem.classList.add('show');
    }

}

// let showAuthOverlay = (event) => {

//     event.preventDefault();
//     event.stopPropagation();

//     var btnPressed = event.currentTarget;
//     var iFrame = btnPressed.parentElement.parentElement.parentElement.querySelectorAll('.iframe-container');

//     iFrame.forEach(element => {

//         element.classList.add('show');

//     });

// }

// obsolete
// let items = document.querySelectorAll('.pnp-item');

// items.forEach(element => {
//     element.addEventListener('click', clickPnpItem, false);
// });

// let addTenantBtns = document.querySelectorAll('.pnp-button');

// addTenantBtns.forEach(element => {
//     element.addEventListener('click', showAuthOverlay)
// })

// let closePanel = (event) => {

//     event.preventDefault();
//     event.stopPropagation();

//     event.currentTarget.classList.remove('show');

// }

// let closeWizards = document.querySelectorAll('.next-step');

// closeWizards.forEach(element => {

//     element.addEventListener('click', closePanel);

// });

let handleBurgerMenu = (event) => {

    console.log('Burger have been clicked');
    event.preventDefault();
    event.stopPropagation();

    let burgerMenu = document.querySelector('.nav-list');

    if (burgerMenu !== null) {

        console.log(burgerMenu);
        if (burgerMenu.classList.contains('show')) {

            burgerMenu.classList.remove('show');

        } else {

            burgerMenu.classList.add('show');

        }

    }

}

const toggleFilter = (event) => {
    event.preventDefault();
    console.log(event.target.classList.contains('active'));
    if (event.target.classList.contains('active') === true) {

        event.target.classList.remove('active');

        let filterPanel = document.querySelector('.pnp-filter-panel');
        console.log(filterPanel.classList);
        filterPanel.classList.remove('show');

    } else {

        event.target.classList.add('active');

        let filterPanel = document.querySelector('.pnp-filter-panel');
        console.log(filterPanel.classList);
        filterPanel.classList.add('show');

    }

}

let burger = document.querySelector('#pnp-btn-burger');
if (burger !== null) {
    burger.addEventListener('click', handleBurgerMenu);
}

let filter = document.querySelector('.pnp-filter .pnp-filter-toggle')
if (filter !== null) {
    filter.addEventListener('click', toggleFilter);
}


const colorPickerInit = () => {

    let colorPickers = document.querySelectorAll("input.pnp-tb[name^=ThemeColor]");

    for (let i = 0; i < colorPickers.length; i++) {

        colorPickers[i].addEventListener("focusin", () => {

            const colorSelector = document.querySelector('.color-picker');
            const header = document.querySelector('.header-img');

            let computedStyles = window.getComputedStyle(header);

            /**
             * Overlay style correction
             */
            if (colorSelector !== null && header !== null) {

                let marginSetOff = computedStyles.getPropertyValue('height').replace('px', '') * -1;

                colorSelector.style.marginTop = marginSetOff + "px";
                colorSelector.style.backgroundColor = "lime";

            }

        });

        // init all color picker
        (new CP(colorPickers[i])).on("change", function (color) {

            this.source.value = '#' + color;

            let sourceId = this.source.id;

            console.log(sourceId);

            let colorPreview = document.querySelector('.pnp-colorpreview[rel=' + sourceId + ']');
            colorPreview.style.backgroundColor = '#' + color;

        })

    }
}

colorPickerInit();
