window.saveAsFile = function (fileName, mimeType, content) {
    var blob = new Blob([content], { type: mimeType });
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};