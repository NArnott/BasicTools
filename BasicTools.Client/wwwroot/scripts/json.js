export function displayJson(element, data) {
    var json = JSON.parse(data);

    $(element).jsonViewer(json);
}