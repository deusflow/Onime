window.showFunnyPopup = function (title, message) {
    const popup = document.createElement("div");
    popup.style.position = "fixed";
    popup.style.top = "50%";
    popup.style.left = "50%";
    popup.style.transform = "translate(-50%, -50%)";
    popup.style.background = "#fff";
    popup.style.border = "3px solid #000";
    popup.style.padding = "20px";
    popup.style.borderRadius = "12px";
    popup.style.boxShadow = "0 0 20px rgba(0,0,0,0.5)";
    popup.style.zIndex = 9999;
    popup.style.fontFamily = "Comic Sans MS, sans-serif";
    popup.style.textAlign = "center";
    popup.innerHTML = `
        <h2>${title}</h2>
        <p>${message}</p>
        <button onclick="this.parentElement.remove()">😎 Ок</button>
    `;
    document.body.appendChild(popup);
};