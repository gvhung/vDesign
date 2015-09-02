var pba = pba || {};

pba.truncateString = function (str, len) {
    if (typeof str !== 'string' || str.length < len)
        return str;

    return str.substr(0, len - 3) + '...';
};

function getSafeImageUrl(obj, path, w, h) {
    return getImageUrl(getValueByString(obj, path), w, h);
}

function getValueByString(obj, path, def) {
    var i, len;

    for (i = 0, path = path.split('.'), len = path.length; i < len; i++) {
        if (!obj || typeof obj !== 'object') return def;
        obj = obj[path[i]];
    }

    if (obj === undefined) return def || obj;
    return obj;
}

function getImageUrl(img, w, h) {
    if (img) {
        return getHostName() + "Files/GetImage?id=" + img.FileID + "&width=" + w + "&height=" + h;
    }

    return getHostName() + "Files/GetImage?id=null";
}

function getHostName() {
    return "http://regulation.pba.su/";
    //return "http://localhost:9998/";
}