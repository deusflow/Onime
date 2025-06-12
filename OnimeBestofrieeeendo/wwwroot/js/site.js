// Тут мы просто храним нужные штуки для работы навигации
let currentSectionIndex = 0; // индекс текущей секции (0 - первая)
let sections = [];           // массив всех секций на странице
let isScrolling = false;     // флаг, чтобы не было двойных переходов
let dotNetHelper = null;     // сюда Blazor передаст свой объект для связи


// Эта функция делает так, чтобы только одна кнопка была "активной"
function updateActiveButton(sectionId) {
    // Находим все кнопки, у которых есть data-section
    const buttons = document.querySelectorAll('button[data-section]');
    // Перебираем их
    buttons.forEach(btn => {
        // Если у кнопки data-section совпадает с нужной секцией — делаем её активной
        if (btn.getAttribute('data-section') === sectionId) {
            btn.classList.add('active');
        } else {
            btn.classList.remove('active');
        }
    });
}


// Эта функция вызывается, когда жмём на кнопку меню
window.scrollToSection = (sectionId) => {
    // Находим нужную секцию по id
    const element = document.getElementById(sectionId);
    // Если секции нет или сейчас уже идёт скролл — ничего не делаем
    if (!element || isScrolling) return;
    isScrolling = true;
    // Запоминаем индекс секции (чтобы работали стрелки)
    const index = sections.findIndex(section => section.id === sectionId);
    if (index !== -1) {
        currentSectionIndex = index;
    }
    // Скроллим к секции
    element.scrollIntoView({ behavior: 'auto', block: 'nearest', inline: 'start' });
    // Сообщаем Blazor, что секция сменилась
    updateActiveSection(sectionId);
    // Подсвечиваем кнопку
    updateActiveButton(sectionId);
    // Через 150мс разрешаем следующий скролл
    setTimeout(() => { isScrolling = false; }, 150);
};


// Эта функция говорит Blazor "мы перешли на другую секцию!"
function updateActiveSection(sectionId) {
    if (dotNetHelper && typeof dotNetHelper.invokeMethodAsync === 'function') {
        dotNetHelper.invokeMethodAsync('UpdateActiveSection', sectionId)
            .catch(err => console.error('[JS updateActiveSection] invokeMethodAsync error:', err));
    }
}


// 6. Просто логирование из C# (чтобы видеть сообщения в консоли)

window.blazorLog = function(message) {
    console.log(message);
};

// Add function to reinitialize scroll behavior
window.reinitializeScrollBehavior = function() {
    console.log('[reinitializeScrollBehavior] Reinitializing scroll behavior for path:', window.location.pathname);
    
    // Remove existing event listeners to avoid duplicates
    const currentPath = window.location.pathname;
    const shouldBlock = currentPath === '/';
    
    if (shouldBlock) {
        console.log('[reinitializeScrollBehavior] Blocking scroll on main page');
    } else {
        console.log('[reinitializeScrollBehavior] Allowing scroll on page:', currentPath);
    }
    
    // Force enable scroll for trading zone pages
    if (currentPath.startsWith('/test') || currentPath.startsWith('/create') || currentPath.startsWith('/details')) {
        document.body.style.overflow = 'auto';
        document.documentElement.style.overflow = 'auto';
        console.log('[reinitializeScrollBehavior] Forced scroll enable for Trading Zone page');
    }
};


// 7. Инициализация при загрузке страницы

document.addEventListener('DOMContentLoaded', function() {
    // Собираем все элементы с классом .section
    sections = Array.from(document.querySelectorAll('.section'));
    // Если есть хотя бы одна секция — подсветим первую кнопку
    if (sections.length > 0) {
        updateActiveButton(sections[0].id);
    }
    // === Блокировка скролла мышью и свайпами для .products-wrapper ===
    var productsWrapper = document.querySelector('.products-wrapper');
    if (productsWrapper) {
        // wheel (мышь, тачпад)
        productsWrapper.addEventListener('wheel', function(e) {
            e.preventDefault();
        }, { passive: false });
        // touchmove (мобильные свайпы)
        productsWrapper.addEventListener('touchmove', function(e) {
            e.preventDefault();
        }, { passive: false });
        // keyboard arrows (опционально)
        productsWrapper.addEventListener('keydown', function(e) {
            if (['ArrowLeft','ArrowRight','PageUp','PageDown','Home','End'].includes(e.key)) {
                e.preventDefault();
            }
        });
    }
    // dotNetHelper Blazor может передать отдельно, если надо

    // Block scroll only on main page (/)
    function shouldBlockScroll() {
        const currentPath = window.location.pathname;
        const shouldBlock = currentPath === '/';
        console.log('[shouldBlockScroll] Current path:', currentPath, 'Should block:', shouldBlock);
        return shouldBlock;
    }

    document.addEventListener('wheel', function(e) {
        if (shouldBlockScroll()) {
            console.log('[wheel] Blocking scroll on main page:', window.location.pathname);
            e.preventDefault();
        } else {
            console.log('[wheel] Allowing scroll on:', window.location.pathname);
        }
    }, { passive: false });

    document.addEventListener('touchmove', function(e) {
        if (shouldBlockScroll()) {
            console.log('[touchmove] Blocking scroll on main page:', window.location.pathname);
            e.preventDefault();
        } else {
            console.log('[touchmove] Allowing scroll on:', window.location.pathname);
        }
    }, { passive: false });

    document.addEventListener('keydown', function(e) {
        if (shouldBlockScroll() && ['ArrowUp', 'ArrowDown', 'PageUp', 'PageDown', 'Home', 'End'].includes(e.code)) {
            console.log('[keydown] Blocking scroll on main page:', window.location.pathname);
            e.preventDefault();
        } else if (!shouldBlockScroll()) {
            console.log('[keydown] Allowing scroll on:', window.location.pathname);
        }
    }, { passive: false });
});
