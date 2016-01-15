
(function () {

    //var menuItems = $("ul.menu li a");
    //menuItems.on("click", function () {
    //    alert("hello");
    //});

    var $sidebarAndWrapper = $("#sidebar,#wrapper");

    $("#sidebarToggle").on("click", function () {
        $sidebarAndWrapper.toggleClass("hide-sidebar");
        if($sidebarAndWrapper.hasClass("hide-sidebar")){
            $(this).text("Show Sidebar");
        } else {
            $(this).text("Hide Sidebar");
        }
    });
})();