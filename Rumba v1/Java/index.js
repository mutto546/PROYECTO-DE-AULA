// Funcion que estaba en index, la coloqué acá
// Todo esto estaba en index:

const usuarioActivo = localStorage.getItem("usuarioActivo");

if (!usuarioActivo) {
    window.location.href = "login.html";
}

function logout() {
    localStorage.removeItem("usuarioActivo");
    window.location.href = "login.html";
}