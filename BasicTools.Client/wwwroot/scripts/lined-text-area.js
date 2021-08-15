export function initialize(canvas, textarea) {
    canvas.height = textarea.clientHeight;

    textarea.paint = function () {
        if (canvas.height != this.clientHeight)
            canvas.height = this.clientHeight; // on resize

        var style = window.getComputedStyle(textarea);
        var lineHeight = parseInt(style.lineHeight);

        var ctx = canvas.getContext("2d");

        ctx.fillStyle = style.backgroundColor;
        ctx.fillRect(0, 0, 48, this.scrollHeight + 1);

        ctx.fillStyle = "#808080";
        ctx.font = style.fontSize + " monospace"; // NOTICE: must match TextArea font-size and lineheight !!!
        var startIndex = Math.floor(this.scrollTop / lineHeight, 0);
        var endIndex = startIndex + Math.ceil(this.clientHeight / lineHeight, 0);
        for (var i = startIndex; i < endIndex; i++) {
            var ph = lineHeight - this.scrollTop + (i * lineHeight);
            var text = '' + (1 + i);  // line number
            ctx.fillText(text, 38 - (text.length * 10), ph);
        }
    }

    textarea.onscroll = function (ev) { this.paint(); };
    textarea.onmousedown = function (ev) { this.mouseisdown = true; }
    textarea.onmouseup = function (ev) { this.mouseisdown = false; this.paint(); };
    textarea.onmousemove = function (ev) { if (this.mouseisdown) this.paint(); };

    textarea.paint();
}