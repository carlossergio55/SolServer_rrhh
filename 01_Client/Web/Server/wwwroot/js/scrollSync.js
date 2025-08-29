export function initScrollSync(top, bottom) {
    if (!top || !bottom) return;

    top.addEventListener("scroll", () => {
        bottom.scrollLeft = top.scrollLeft;
    });

    bottom.addEventListener("scroll", () => {
        top.scrollLeft = bottom.scrollLeft;
    });
}