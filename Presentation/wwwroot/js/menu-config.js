const menuSections = {
    "Home": ["home"],
    "Job": ["job", "jobreport"],
    "BaseInfo": ["user", "role", "customer", "event", "plan"],
};
const initActiveMenu = () => {
    // === following js will activate the menu in left side bar based on url ====
    let sectionSelectedClass = "";
    const mainPageUrl = window.location.href.split("/")
    const pageControllerUrl = mainPageUrl[3];
    const pageActionUrl = mainPageUrl[4]
    const finalControllerPageUrl = pageControllerUrl.split("?")[0]
    const queryStrings = new URLSearchParams(window.location.search)
    
    if (menuSections["Home"].includes(finalControllerPageUrl.toLowerCase())) {
        $("#MainHome").addClass("active");
        sectionSelectedClass = ".config-active-menu-home"
    }
    if (menuSections["BaseInfo"].includes(finalControllerPageUrl.toLowerCase())) {
        $("#MainBaseInfo").addClass("active");
        sectionSelectedClass = ".config-active-menu-baseInfo"
    }
    if (menuSections["Job"].includes(finalControllerPageUrl.toLowerCase())) {
        $("#MainJob").addClass("active");
        sectionSelectedClass = ".config-active-menu-job"
    }

    $(`${sectionSelectedClass} .menu-item`).each(function () {
        
        const mainPageHref = this.href.split("/");
        const aController = mainPageHref[3]
        const aMethod = mainPageHref[4] ?? ""

        
        if (aController === finalControllerPageUrl && (aMethod === pageActionUrl || ((aMethod === "") && ["Create", "Edit"].includes(pageActionUrl)))) {
            $(this).addClass("active");
            let secondLevel = $(this).parents(".nav-second-level");
            if(secondLevel.length > 0){
                secondLevel = secondLevel[0]
                $(secondLevel).parent().children("a").addClass("active");
            }
            $(this).parent().parent().addClass("active mm-show");
            $(this).parents(".main-icon-menu-pane").addClass("active")
            // $(`#${sectionId}`).addClass("active")
            // $(`#${parentId}`).addClass("active")
        }
    });
}
$(document).ready(function () {
    initActiveMenu();
})
