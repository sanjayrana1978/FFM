$(document).ready(function () {
    $(".as-easy-tips").click(function () {
        $(this).toggleClass('active');
        $(".as-easy-data").slideToggle();
        $(".header").toggleClass('active');
    });
    $(".tips-btn").click(function () {
        if ($(".tips-btn").hasClass(".open-close-tip")) {
            $(this).html("OPEN TIPS");
        }
        else {
            $(this).html("CLOSE TIPS");
        }
        $(".tips-btn").toggleClass(".open-close-tip");
    });


    $(".see-more").click(function () {
        if ($(".see-more").hasClass(".show-more")) {
            $(this).html("SEE MORE");
        }
        else {
            $(this).html("SEE LESS");
        }
        $(".see-more").toggleClass(".show-more");
    });

    /*use for see more less*/
    const counter = $(".featured-product-grid .item-grid").attr("data-item-count"),
        parentclass = $(".featured-product-grid .item-grid"),
        seebtn = $(".see-more");
        if ($(window).width() >= 1601 && counter > 5) {
            $(".see-more").addClass("show-more");
            removeclass();
        }
        else if ($(window).width() >= 1281 && $(window).width() <= 1600 && counter > 4) {
            $(".see-more").addClass("show-more");
            removeclass();
        }
        else if ($(window).width() >= 481 && $(window).width() <= 1280 && counter > 3) {
            $(".see-more").addClass("show-more");
            removeclass();
        }
        else if ($(window).width() <= 480 && counter > 2) {
            $(".see-more").addClass("show-more");
            removeclass();
        }
        function removeclass() {
            seebtn.click(function () {
                parentclass.toggleClass("more-than-count");
            });
        }

    //Custom Style for responsive bar
    var j = localStorage.getItem("popup");
    if (j == "true") {
        localStorage.setItem("popup", "true");
        $('.master-wrapper-page > .topic-block:first-child').css('display', 'none');
        $('.master-wrapper-page > .topic-block:first-child').removeClass('active_header_alert');
        $('.admin-header-links').removeClass('active_adminlink');
    }
    else {
        $('.master-wrapper-page > .topic-block:first-child').css('display', 'block');
        $('.master-wrapper-page > .topic-block:first-child').addClass('active_header_alert');
        $('.admin-header-links').addClass('active_adminlink');
    }
    $('.alert-close').click(function (e) {
        localStorage.setItem("popup", "true");
        $('.master-wrapper-page > .topic-block:first-child').css('display', 'none');
        $('.master-wrapper-page > .topic-block:first-child').removeClass('active_header_alert');
        $('.admin-header-links').removeClass('active_adminlink');
    });

    //use code for product details page
    $(".price-qty-wrapper .prices").after($(".add-to-cart-qty-wrapper"));
    $(".price-qty-wrapper .prices").after($(".qty-dropdown"));
    $(".product-prices-box").after($(".full-description"));
    $(".full-description").after($(".product-collateral"));

    //use code for gallery add to cart
    $(".gallery .gallery-addtocart").click(function () {
        $(".overview .add-to-cart-button").trigger("click");
    });

    //var stickyEl = new Sticksy('.gallery.js-sticky-widget', {
    //    topSpacing: 60,
    //})
    //stickyEl.onStateChanged = function (state) {
    //    if (state === 'fixed') stickyEl.nodeRef.classList.add('gallerysticky')
    //    else stickyEl.nodeRef.classList.remove('gallerysticky')
    //}

    //Used For Ui Model
    //$(".shipping-addresses .edit-button").click(function () {
    //    $("body").addClass("overflow-active");
    //    uiClose();
    //});
    //function uiClose() {
    //    $(".html-checkout-page .ui-dialog-titlebar-close").click(function () {
    //        $("body").removeClass("overflow-active");
    //    });
    //}
});
$(window).resize(function () {
    /*use for see more less*/
    const counter = $(".featured-product-grid .item-grid").attr("data-item-count"),
        parentclass = $(".featured-product-grid .item-grid"),
        seebtn = $(".see-more");
    if ($(window).width() >= 1601 && counter > 5) {
        $(".see-more").addClass("show-more");
        removeclass();
    }
    else if ($(window).width() >= 1281 && $(window).width() <= 1600 && counter > 4) {
        $(".see-more").addClass("show-more");
        removeclass();
    }
    else if ($(window).width() >= 481 && $(window).width() <= 1280 && counter > 3) {
        $(".see-more").addClass("show-more");
        removeclass();
    }
    else if ($(window).width() <= 480 && counter > 2) {
        $(".see-more").addClass("show-more");
        removeclass();
    }
    function removeclass() {
        seebtn.click(function () {
            parentclass.toggleClass("more-than-count");
        });
    }
});



//$(window).scroll(function () {
//    var sticky = $('.product-details-page .gallery'),
//        scroll = $(window).scrollTop();

//    if (scroll >= 100) sticky.addClass('fixed');
//    else sticky.removeClass('fixed');
//});

//var stickyOffset = $('.product-details-page .gallery').offset().top;

//$(window).scroll(function () {
//    var sticky = $('.product-details-page .gallery'),
//        scroll = $(window).scrollTop();

//    if (scroll >= stickyOffset) sticky.addClass('fixed');
//    else sticky.removeClass('fixed');
//});
