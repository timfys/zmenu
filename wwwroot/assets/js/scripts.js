document.addEventListener("DOMContentLoaded", function(){
  document.querySelector("body").addEventListener("click", function(e) {
    if(window.MobileCombos instanceof Array && !Array.from(e.target.classList).find(x => x.includes("iti"))){
      window.MobileCombos.forEach(x => {
        CollapseMobileCombo(x.Name);
      });
    }
  })
  if(window.MobileCombos instanceof Array){
    window.MobileCombos.forEach(x => {
      let parent = document.getElementById(x.Name).parentElement;

      parent.querySelector(".iti__dropdown-content").style.width = `${parent.getBoundingClientRect().width}px`;
    });
  }
})
function ExecuteDelegates(delegates){
  for(let i = 0; i < delegates.length; i++){
    delegates[i]();
  }
}
function GetMobileComboSelectedCountry(name){

  let obj = window.MobileCombos.find(x => x.Name === name);

  return {
    CountryIso : obj.SelectedCountry,
    CountryCode : obj.SelectedCountryCode
  }
}
function SetMobileComboSelectedCountryByCode(name, code){
  let parent = document.getElementById(name).parentElement;
  let nodeList = Array.from(parent.querySelector(".iti__country-container").querySelector(".iti__country-list").querySelectorAll("li"));

  let countryIso = nodeList.find(x => x.getAttribute("data-dial-code") === code).getAttribute("data-country-code");

  SetMobileComboSelectedCountry(name, countryIso);

}
function SetMobileComboSelectedCountry(name, countryIso){
  let parent = document.getElementById(name).parentElement;
  let nodeList = Array.from(parent.querySelector(".iti__country-container").querySelector(".iti__country-list").querySelectorAll("li"));

  let selectedNode = nodeList.find(x => x.getAttribute("data-country-code") === countryIso);

  let code = selectedNode.getAttribute("data-dial-code");
  let countryName = selectedNode.querySelector(".iti__country-name").textContent;

  let arrIndex = window.MobileCombos.findIndex(x => x.Name === name);

  window.MobileCombos[arrIndex].SelectedCountry = countryIso;
  window.MobileCombos[arrIndex].SelectedCountryCode = code;

  let button = document.getElementById(name).parentElement.querySelector(".iti__country-container").querySelector(".iti__selected-country");
  let flagElem = button.querySelector(".iti__flag");
  let dialCodeElem = button.querySelector(".iti__selected-dial-code");

  button.setAttribute("title", countryName);

  Array.from(flagElem.classList).forEach((x) => {
    if(x !== "iti__flag" && x.includes("iti"))
      flagElem.classList.remove(x);
  });
  flagElem.classList.add(`iti__${countryIso.toLowerCase()}`);

  flagElem.querySelector(".iti__a11y-text").textContent = `${countryName} +${code}`;
  dialCodeElem.textContent = `+${code}`;

}
function ToggleMobileCombo(name) {
  let button = document.getElementById(name).parentElement.querySelector(".iti__country-container").querySelector(".iti__selected-country");

  if(button.getAttribute("aria-expanded") === "true"){
    CollapseMobileCombo(name);
  }else{
    ExpandMobileCombo(name);
  }
}
function ExpandMobileCombo(name){

  let parent = document.getElementById(name).parentElement;
  let button = parent.querySelector(".iti__country-container").querySelector(".iti__selected-country");

  let content = parent.querySelector(".iti__dropdown-content");

  content.style.width = `${parent.getBoundingClientRect().width}px`;
  content.classList.remove("iti__hide");
  content.style.display = "block";

  button.setAttribute("aria-expanded", "true");

  parent.querySelector(".iti__search-input").focus();

}

function CollapseMobileCombo(name){

  let parent = document.getElementById(name).parentElement;
  let button = parent.querySelector(".iti__country-container").querySelector(".iti__selected-country");

  let content = parent.querySelector(".iti__dropdown-content");

  content.classList.add("iti__hide");
  content.style.display = "none";

  button.setAttribute("aria-expanded", "false");


}

function SearchMobileComboCountries(name, keyword){
  let parent = document.getElementById(name).parentElement;
  let elems = parent.querySelector(".iti__country-list").querySelectorAll(".iti__country");

  if(!keyword)
    elems.forEach(x => {x.style.display = "flex"});
  else{
    keyword = keyword.toLowerCase();
    elems.forEach(x => {
      let code = x.getAttribute("data-dial-code").toLowerCase();
      let countryIso = x.getAttribute("data-country-code").toLowerCase();
      let countryName = x.querySelector(".iti__country-name").textContent.toLowerCase();

      x.style.display = code.includes(keyword) || countryIso.includes(keyword) || countryName.includes(keyword) || `+${code}`.includes(keyword) ? "flex" : "none";
    })
  }

}

function SanitizeMobileInput(value, maxLength) {
  let filtered = value.replace(/\D/g, "");
  return filtered.slice(0, maxLength);
}

function OnMobileInput(e, name) {
  let el = document.getElementById(name);
  let maxLength = 13;

  let cleaned = SanitizeMobileInput(el.value, maxLength);

  if (el.value !== cleaned) {
    let cursorPos = el.selectionStart;
    el.value = cleaned;
    el.setSelectionRange(cleaned.length, cleaned.length);
  }
}

function OnMobilePaste(e, name) {
  e.preventDefault();
  let el = document.getElementById(name);
  let maxLength = 13;

  let paste = (e.clipboardData || window.clipboardData).getData('text') || '';
  let sanitized = SanitizeMobileInput(paste, maxLength);

  let start = el.selectionStart;
  let end = el.selectionEnd;
  let value = el.value;

  let newValue = value.slice(0, start) + sanitized + value.slice(end);
  let cleaned = SanitizeMobileInput(newValue, maxLength);

  el.value = cleaned;

  let newCursor = Math.min(start + sanitized.length, cleaned.length);
  el.setSelectionRange(newCursor, newCursor);

  let parent = document.getElementById(name).parentElement;
  let elems = Array.from(parent.querySelector(".iti__country-list").querySelectorAll(".iti__country"));

  let elem = elems.find(x => {

    let dialCode = x.getAttribute("data-dial-code");

    return sanitized.startsWith(dialCode) && dialCode.length < sanitized.length;
  });

  if(elem){

    let countryIso = elem.getAttribute("data-country-code");
    let dialCode = elem.getAttribute("data-dial-code");

    el.value = newValue.substring(dialCode.length);
    SetMobileComboSelectedCountry(name, countryIso);
  }
}

function DisplayLoadingScreen(){

  let elem = document.querySelector("#loading-screen");
  let htmlElem = document.querySelector("html");

  if(!elem){
    elem = document.createElement("div");
    elem.innerHTML = `<div id="loader-box">
            <p>${document.querySelector("#please-wait-text").value}...</p>
            <img src="/assets/images/Double Ring-1s-200px.svg" alt="loader" style="width: 20px;">
        </div>`
    elem.id = "loading-screen";
    document.querySelector("#please-wait-text").remove();
    htmlElem.appendChild(elem);
  }

  elem.style.display = "block";
  htmlElem.style.pointerEvents = "none";
}
function CloseLoadingScreen(){

  let elem = document.querySelector("#loading-screen");
  let htmlElem = document.querySelector("html");

  if(!elem){
    elem = document.createElement("div");
    elem.innerHTML = `<div id="loader-box">
            <p>${document.querySelector("#please-wait-text").value}...</p>
            <img src="/assets/images/Double Ring-1s-200px.svg" alt="loader" style="width: 20px;">
        </div>`
    elem.id = "loading-screen";
    document.querySelector("#please-wait-text").remove();
    htmlElem.appendChild(elem);
  }

  elem.style.display = "none";
  htmlElem.style.pointerEvents = "initial";
}



function deleteCookie(name) {
  document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
}

function setCookie(name, value, days) {
  let expires = "";
  if (days) {
    const date = new Date();
    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
    expires = "; expires=" + date.toUTCString();
  }
  document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

document.addEventListener("DOMContentLoaded", function() {
  let path = window.location.pathname;
  
  if(path.startsWith("/he")) {
    setCookie("Culture", "he");
  }
  else if(path.startsWith("/ru")) {
    setCookie("Culture", "ru");
  }else {
    setCookie("Culture", "en");
  }
})

const init = async () => {

  // Mobile Menu //
  let burgerButton = document.getElementById("burgerButton");
  let mobileNav = document.querySelector(".mobile__nav");
  let body = document.body;

  function closeMenu() {
    mobileNav.classList.remove("nav--active");
    body.classList.remove("lock");
  }

  if (burgerButton && mobileNav) {
    let closeBtn = mobileNav.querySelector("#closeMenu");
    let links = mobileNav.querySelectorAll(".navigation__list a");
    let nav = mobileNav.querySelector("nav");

    // открыть меню
    burgerButton.addEventListener("click", function (e) {
      e.stopPropagation();
      mobileNav.classList.add("nav--active");
      body.classList.add("lock");
    });

    // закрыть по крестику
    if (closeBtn) closeBtn.addEventListener("click", closeMenu);

    // закрыть по клику на ссылку
    // закрыть по клику на ссылку
    links.forEach(link => {
      link.addEventListener("click", function (e) {
        // проверяем родителя
        let parentLi = link.closest("li");

        if (parentLi && parentLi.classList.contains("menu-item-has-children")) {
          // у ссылки есть подменю → не закрываем
          e.preventDefault(); // чтобы не переходило по ссылке сразу, если нужно открыть подменю
        } else {
          closeMenu();
        }
      });
    });


    // закрыть по клику вне nav
    mobileNav.addEventListener("click", function (e) {
      if (!nav.contains(e.target)) {
        closeMenu();
      }
    });
  }


  // Fixed Header //
  const header = document.querySelector(".header");
  const main = document.querySelector(".main");

  if (header && main) {
    const headerH = header.offsetHeight; // Получаем высоту хедера
    const checkScroll = (scrollOffset) => {
      if (scrollOffset >= headerH) {
        header.classList.add("fixed");
        main.style.paddingTop = `${headerH}px`; // Устанавливаем верхний отступ
      } else {
        header.classList.remove("fixed");
        main.style.paddingTop = `0`; // Убираем верхний отступ
      }
    };

    let scrollOffset = window.scrollY; // Начальное значение прокрутки
    checkScroll(scrollOffset);

    window.addEventListener("scroll", () => {
      scrollOffset = window.scrollY;
      checkScroll(scrollOffset);
    });
  }
  // Fixed Header //
};


// -has-children
document.addEventListener("DOMContentLoaded", function () {
  // Находим все элементы меню с подменю
  const menuItems = document.querySelectorAll(".menu-item-has-children > a");

  // Обрабатываем клик по каждому элементу меню
  menuItems.forEach(item => {
    item.addEventListener("click", function (e) {
      e.preventDefault(); // Предотвращаем переход по ссылке

      const parentMenuItem = item.parentElement; // Получаем родителя элемента <a>

      // Если у родителя уже есть класс active, то оставляем его, иначе убираем у соседей и добавляем текущему
      if (!parentMenuItem.classList.contains('active')) {
        // Убираем класс active у всех соседей
        parentMenuItem.parentElement.querySelectorAll('.menu-item-has-children').forEach(sibling => {
          if (sibling !== parentMenuItem) {
            sibling.classList.remove('active');
          }
        });

        // Добавляем класс active к текущему элементу
        parentMenuItem.classList.add('active');
      } else {
        // Если уже активен, то просто убираем класс
        parentMenuItem.classList.remove('active');
      }
    });
  });

  // Закрытие подменю при клике вне области навигации
  document.addEventListener("click", function (e) {
    // Если клик не по элементу с подменю
    if (!e.target.closest(".menu-item-has-children")) {
      // Убираем класс 'active' у всех родительских элементов меню
      document.querySelectorAll('.menu-item-has-children.active').forEach(activeItem => {
        activeItem.classList.remove("active");
      });
    }
  });
});
// -has-children


// Динамическая смена фона в секциях
function updateBackgrounds() {
  const isMobile = window.innerWidth <= 560;

  document.querySelectorAll("section").forEach(section => {
    const desktopBg = section.getAttribute("data-desktop-bg");
    const mobileBg = section.getAttribute("data-mobile-bg");

    if (isMobile && mobileBg) {
      section.style.backgroundImage = `url(${mobileBg})`;
    } else {
      // Если desktopBg есть — ставим его, если нет — убираем фон
      section.style.backgroundImage = desktopBg ? `url(${desktopBg})` : "none";
    }
  });
}

document.addEventListener("DOMContentLoaded", updateBackgrounds);
window.addEventListener("resize", updateBackgrounds);
// Динамическая смена фона в секциях

//swiper
document.addEventListener("DOMContentLoaded", function () {
  // Инициализация слайдера "customer"
  if (document.querySelector("#news")) {
    new Swiper("#news", {
      observer: true,
      observeParents: true,
      loop: true,
      // autoplay: {
      //   delay: 3000,
      //   disableOnInteraction: false,
      // },
      pagination: {
        el: ".news-pagination",
        clickable: true,
      },
      navigation: {
        nextEl: ".news-button-next",
        prevEl: ".news-button-prev",
      },
      breakpoints: {
        320: {
          slidesPerView: 1.2, // Один полный слайд и куски по бокам
          spaceBetween: 20, // Расстояние между слайдами
          centeredSlides: true,
        },
        560: {
          slidesPerView: 1.5, // Один полный слайд и куски по бокам
          centeredSlides: true,
          spaceBetween: 20, // Расстояние между слайдами
        },
        768: {
          slidesPerView: 2, // Один полный слайд и куски по бокам
          centeredSlides: false,
        },
        1024: {
          slidesPerView: 3,
          spaceBetween: 30,
        },
      },
    });
  }

  if (document.querySelector("#reviews")) {
    new Swiper("#reviews", {
      observer: true,
      observeParents: true,
      loop: true,
      autoplay: {
        delay: 3000,
        disableOnInteraction: false,
      },
      pagination: {
        el: ".reviews-pagination",
        clickable: true,
      },
      navigation: {
        nextEl: ".reviews-button-next",
        prevEl: ".reviews-button-prev",
      },
      breakpoints: {
        320: {
          slidesPerView: 1, // Один полный слайд и куски по бокам
          spaceBetween: 20, // Расстояние между слайдами
        },
        440: {
          slidesPerView: 1.5, // Один полный слайд и куски по бокам
          spaceBetween: 18, // Расстояние между слайдами
        },
        560: {
          slidesPerView: 2, // Один полный слайд и куски по бокам
          spaceBetween: 20, // Расстояние между слайдами
        },
        900: {
          slidesPerView: 3, // Один полный слайд и куски по бокам
          spaceBetween: 20, // Расстояние между слайдами

        },
        1100: {
          slidesPerView: 3.5,
          spaceBetween: 20, // Расстояние между слайдами
        },
      },
    });
  }

});
// swiper

//faq collapse
$(document).ready(function () {
  // Обработчик клика на элемент с классом faq__title
  $(".action").on("click", function () {
    // Находим ближайший родительский элемент с классом faq__item
    var $item = $(this).closest(".faq__item");
    // Переключаем класс active у найденного элемента
    $item.toggleClass("active");
  });

  // Обработчик клика на элемент с классом faq__btn
  $(".faq__btn").on("click", function () {
    // Находим ближайший родительский элемент с классом faq__item
    var $item = $(this).closest(".faq__item");
    // Переключаем класс active у найденного элемента
    $item.toggleClass("active");
  });
});
//faq collapse

// Инициализация после загрузки страницы
window.onload = init;