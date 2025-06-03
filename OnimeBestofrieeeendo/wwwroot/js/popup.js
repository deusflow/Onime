window.showFunnyPopup = function (title, message) {
    // Удаляем старую модалку если она уже открыта
    const oldModal = document.querySelector(".modal-overlay");
    if (oldModal) oldModal.remove();

    // Создаем оверлей
    const overlay = document.createElement("div");
    overlay.className = "modal-overlay";

    // Создаем контент
    const content = document.createElement("div");
    content.className = "modal-content";

    // Добавляем внутреннюю разметку с шутливым стилем
    content.innerHTML = `
    <button class="close-btn" onclick="this.closest('.modal-overlay').remove()">×</button>
    <div class="modal-title">🍜 ${title}</div>
    <p style="font-size: 1.1rem; text-align: center;">${message}</p>
    <div style="display: flex; justify-content: center; margin-top: 25px;">
        <button style="
            background: linear-gradient(to right, #ff66cc, #66ccff);
            color: white;
            font-size: 1rem;
            padding: 10px 20px;
            border: none;
            border-radius: 12px;
            cursor: pointer;
            box-shadow: 0 5px 15px rgba(0,0,0,0.3);
            transition: transform 0.2s;
        " 
        onmouseover="this.style.transform='scale(1.1)'"
        onmouseout="this.style.transform='scale(1)'"
        onclick="this.closest('.modal-overlay').remove()">✨Oh yeah, I totally get it now…</button>
    </div>
    `;

    overlay.appendChild(content);
    document.body.appendChild(overlay);
};