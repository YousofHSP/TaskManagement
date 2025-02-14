const menuSections = {
    "General": ["user", "role", "job", "customer", "event", "plan"]
};
const initActiveMenu = () => {
    // === following js will activate the menu in left side bar based on url ====
    let sectionSelectedClass = "";
    const mainPageUrl = window.location.href.split("/")
    const pageControllerUrl = mainPageUrl[3];
    const pageActionUrl = mainPageUrl[4]
    const finalControllerPageUrl = pageControllerUrl.split("?")[0]
    const queryStrings = new URLSearchParams(window.location.search)
    
    // if (menuSections["General"].includes(finalControllerPageUrl.toLowerCase())) {
        // $("#MainHome").addClass("active");
        // sectionSelectedClass = "config-active-menu-home"
    // }

    $(`.menu-item`).each(function () {
        
        const mainPageHref = this.href.split("/");
        const pageControllerHref = mainPageHref[3]
        const pageActionsHref = mainPageHref[4]
        
        // const parentId = $(this).data("parentId")
        // const sectionId = $(this).data("sectionId")
        
        if (finalPageHref === finalPageUrl || (otherController !== undefined && otherController.includes(finalControllerPageUrl))) {
            
            $(this).addClass("active");
            $(this).parent().parent().addClass("active mm-show");
            $(this).parent().parent().parent().addClass("active");
            // $(`#${sectionId}`).addClass("active")
            // $(`#${parentId}`).addClass("active")
            // $(this).parent().parent().addClass("in");
            // $(this).parent().parent().addClass("show");
            // $(this).parent().parent().prev().addClass("show");
            // $(this).parent().parent().parent().addClass("show");
            // $(this).parent().parent().parent().parent().addClass("show");
            // $(this).parent().parent().parent().parent().parent().addClass("show");
            // $(this).parent().parent().parent().parent().parent().parent().addClass("show");
        }
    });
}
$(document).ready(function () {
    initActiveMenu();
})