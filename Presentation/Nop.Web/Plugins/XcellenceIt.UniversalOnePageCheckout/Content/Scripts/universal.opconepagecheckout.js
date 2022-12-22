/*
** Universal one page checkout
*/
window.CommonDefaults =
{
    shippingRequired: true
};

$(document).ready(function () {

    $(document).on('keypress', function (e) {
        if (e.keyCode === 13) {
            $("#shopping-cart-form").submit(function (e) {
                e.preventDefault();
            });
        }
    });

    // Shopping cart item event START
    $(document).on("change", ".qty-input,.qty-dropdown", function () {
        var form = $("#shopping-cart-form").serialize();
        updateShoppingCartItem(form);
    });
    $(document).on("click", "a[name='removefromcart']", function () {
        var form = 'removefromcart=' + $(this).attr('value');
        updateShoppingCartItem(form);
    });
    // Shopping cart item event END

    // Billing address drop-down change
    $(document).on("change", "#billing-address-select", function () {
        var currentSelected = $(this).find(':selected')[0];
        Billing.addressChange(currentSelected);
    });
    // Shipping address drop-down change
    $(document).on("change", "#shipping-address-select", function () {
        var currentSelected = $(this).find(':selected')[0];
        Shipping.addressChange(currentSelected);
    });
});

//Update Shopping cart item 
function updateShoppingCartItem(formData) {
    formData = formData + "&" + $('#checkout-step-checkoutAttributes-form').serialize();
    displayAjaxLoading(true);
    $.ajax({
        url: "/OPCShoppingCart/OPCUpdateCart",
        type: "POST",
        cache: false,
        data: formData,
        success: function (response) {
            if (response.returnUrl) {
                if (response.returnUrl.length > 0)
                    window.location.href = response.returnUrl;
            }
            else
                if (response) {
                    $("#checkout-step-confirm-order").html(response.carthtml);
                    $(".header-links .cart-qty").html(response.updatetopcartsectionhtml);
                    $("#flyout-cart").html(response.updateflyoutcartsectionhtml);
                    // ShippingMethod.save();
                    // Changes the method for cheking as while remove or update the product we are getting issue of shipping not found, so on remove or update we are saving the shipping and display again shipping method
                    Shipping.save();
                }
        },
        error: function () {
            alert("Failed to update cart item.");
        }
    });
}

var Checkout = {
    loadWaiting: false,
    failureUrl: false,

    init: function (failureUrl) {
        this.loadWaiting = false;
        this.failureUrl = failureUrl;
    },

    ajaxFailure: function () {
        location.href = Checkout.failureUrl;
        $('.confirm-order-next-step-button').hide();
        displayAjaxLoading(false);
    },

    _disableEnableAll: function (element, isDisabled) {
        var descendants = element.find('*');
        $(descendants).each(function () {
            if (isDisabled) {
                $(this).prop("disabled", true);
            } else {
                $(this).prop("disabled", false);
            }
        });

        if (isDisabled) {
            element.prop("disabled", true);
        } else {
            $(this).prop("disabled", false);
        }
    },

    setLoadWaiting: function (step, keepDisabled) {
        if (step) {
            if (this.loadWaiting) {
                this.setLoadWaiting(false);
            }
            var container = $('#' + step + '-buttons-container');
            container.addClass('disabled');
            container.css('opacity', '.5');
            this._disableEnableAll(container, true);
            $('#' + step + '-please-wait').show();
        } else {
            if (this.loadWaiting) {
                var container = $('#' + this.loadWaiting + '-buttons-container');
                var isDisabled = (keepDisabled ? true : false);
                if (!isDisabled) {
                    container.removeClass('disabled');
                    container.css('opacity', '1');
                }
                this._disableEnableAll(container, isDisabled);
                $('#' + this.loadWaiting + '-please-wait').hide();
            }
        }
        this.loadWaiting = step;
    },

    gotoSection: function (section) {
        section = $('#opc-' + section);
        section.addClass('allow');
    },

    back: function () {
        //if (this.loadWaiting) return;
        //Accordion.openPrevSection(true, true);
    },

    setStepResponse: function (response) {
        if (response.update_section) {
            $('#checkout-' + response.update_section.name + '-load').html(response.update_section.html);
        }
        if (response.allow_sections) {
            response.allow_sections.each(function (e) {
                $('#opc-' + e).addClass('allow');
            });
        }

        //TODO move it to a new method
        if ($("#billing-address-select").length > 0) {
            Billing.newAddress(!$('#billing-address-select').val());
        }
        if ($("#shipping-address-select").length > 0) {
            Shipping.newAddress(!$('#shipping-address-select').val());
        }

        if (response.goto_section) {
            Checkout.gotoSection(response.goto_section);
            return true;
        }
        if (response.redirect) {
            location.href = response.redirect;
            return true;
        }
        return false;
    }
};





var Billing = {
    form: false,
    saveUrl: false,
    disableBillingAddressCheckoutStep: false,

    init: function (form, saveUrl, disableBillingAddressCheckoutStep) {
        this.form = form;
        this.saveUrl = saveUrl;
        this.disableBillingAddressCheckoutStep = disableBillingAddressCheckoutStep;
    },

    newAddress: function (isNew) {
        if (isNew) {
            this.resetSelectedAddress();
            $('#billing-new-address-form').show();
            $('.confirm-order-next-step-button').hide();
        } else {
            $('#billing-new-address-form').hide();
        }
        $(document).trigger({ type: "onepagecheckout_billing_address_new" });
    },

    toggleShipToSameAddress: function () {
        var curBillingaddressId = $('#billing-address-select').val();
        if (curBillingaddressId > 0) {
            displayAjaxLoading(true);
            this.save();
        } else {
            alert("Please select Billing Address or add new address")
        }
    },

    resetSelectedAddress: function () {
        var selectElement = $('#billing-address-select');
        if (selectElement) {
            selectElement.val('');
        }
        $(document).trigger({ type: "onepagecheckout_billing_address_reset" });
    },

    save: function () {
        if (Checkout.loadWaiting != false) return;

        Checkout.setLoadWaiting('billing');

        $.ajax({
            cache: false,
            async: true,
            url: this.saveUrl,
            data: $(this.form).serialize(),
            type: 'post',
            success: this.nextStep,
            complete: this.resetLoadWaiting,
            error: Checkout.ajaxFailure
        });
    },

    saveWithoutNextStep: function () {
        if (Checkout.loadWaiting != false) return;

        Checkout.setLoadWaiting('billing');

        $.ajax({
            cache: false,
            async: true,
            url: this.saveUrl,
            data: $(this.form).serialize(),
            type: 'post',
            //success: ShippingMethod.save(),
            complete: this.resetLoadWaiting,
            error: Checkout.ajaxFailure
        });
    },
    saveNewAddress: function () {
        displayAjaxLoading(true);
        $.ajax({
            cache: false,
            async: true,
            url: '/opccheckout/OPCSaveNewBillingAddress',
            data: $(this.form).serialize(),
            type: 'post',
            success: this.updateBillingSection,
            complete: this.resetLoadWaiting,
            error: Checkout.ajaxFailure
        });
    },
    updateBillingSection: function (response) {
        // Update billing section
        if (response.update_section) {
            $('#checkout-' + response.update_section.name + '-load').html(response.update_section.html);
        }
        // Set newly created address 
        if (response.newAddressId > 0) {
            Billing.newAddress(false);
            $('#billing-address-select').val(response.newAddressId);
            Billing.save(); // newly added address load into shipping section.
        }
        displayAjaxLoading(false);
    },
    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
        var curBillingaddressId = $('#billing-address-select').val();
        if ($('#ShipToSameAddress').is(':checked')) {
            $('#opc-shipping').hide();

            //fix Bug #42513 (Issue with UPS...)
            //append new billing option before last(New address) option.
            if ($('#shipping-address-select').length > 0) {
                //do not append if already exist
                var optionExists = ($("#shipping-address-select option[value='" + curBillingaddressId + "']").length > 0);
                //alert(optionExists);
                if (!optionExists) {
                    var newAddressText = $('#billing-address-select option:selected').text();
                    //append 
                    $('#shipping-address-select option:last').before($('<option></option>').val(curBillingaddressId).html(newAddressText));
                }
            }
            $('#shipping-address-select').val(curBillingaddressId).change(); // set billing address in shipping & trigger change event to update shiping rates
            //$('#shipping-address-select').val(curBillingaddressId); // set billing address in shipping

            if ($('#billing-address-select').length > 0) {
                // Unchecked pickup point. so if shipping method section visibility work.
                $('#PickUpInStore').prop('checked', false);
            }
        }
        else {
            $('#opc-shipping').show();
        }

        if (!$('#ShipToSameAddress').is(':checked') && window.CommonDefaults.shippingRequired) {
            Shipping.save();
        }
        else
            if ($('#ShipToSameAddress').is(':checked') && $('#billing-address-select').length > 0) {
                ShippingMethod.save();
            }
            else
                if (!window.CommonDefaults.shippingRequired) {
                    PaymentMethod.save();
                }

    },

    nextStep: function (response) {
        //ensure that response.wrong_billing_address is set
        //if not set, "true" is the default value
        if (typeof response.wrong_billing_address == 'undefined') {
            response.wrong_billing_address = false;
        }
        //if (Billing.disableBillingAddressCheckoutStep) {
        //    if (response.wrong_billing_address) {
        //        Accordion.showSection('#opc-billing');
        //    } else {
        //        Accordion.hideSection('#opc-billing');
        //    }
        //}


        if (response.error) {
            if ((typeof response.message) == 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }
            $('.confirm-order-next-step-button').hide();
            displayAjaxLoading(false);
            return false;
        }

        Checkout.setStepResponse(response);


    },

    addressChange: function (data) {
        if (data.value) {
            displayAjaxLoading(true);
            this.saveWithoutNextStep();
        }
    }
};



var Shipping = {
    form: false,
    saveUrl: false,
    init: function (form, saveUrl) {
        this.form = form;
        this.saveUrl = saveUrl;
    },

    newAddress: function (isNew, addressId) {
        if (isNew) {
            this.resetSelectedAddress();
            $('#shipping-new-address-form').show();
            $('.confirm-order-next-step-button').hide();
        } else {
            $('#shipping-new-address-form').hide();
            $("#shipping_address_id").val(addressId)
            this.save();
        }
        $(document).trigger({ type: "onepagecheckout_shipping_address_new" });
    },

    togglePickUpInStore: function () {
        displayAjaxLoading(true);
        this.save();
    },

    resetSelectedAddress: function () {
        var selectElement = $('#shipping-address-select');
        if (selectElement) {
            selectElement.val('');
        }
        $(document).trigger({ type: "onepagecheckout_shipping_address_reset" });

    },

    save: function () {
        if (Checkout.loadWaiting != false) return;

        Checkout.setLoadWaiting('shipping');

        $.ajax({
            cache: false,
            async: true,
            url: this.saveUrl,
            data: $(this.form).serialize(),
            type: 'post',
            success: this.nextStep,
            complete: this.resetLoadWaiting,
            error: Checkout.ajaxFailure
        });
    },
    saveNewAddress: function () {
        displayAjaxLoading(true);
        $.ajax({
            cache: false,
            async: true,
            url: '/opccheckout/OPCSaveNewShippingAddress',
            data: $(this.form).serialize(),
            type: 'post',
            success: this.updateShippingSection,
            complete: this.resetLoadWaiting,
            error: Checkout.ajaxFailure
        });
    },
    updateShippingSection: function (response) {
        // Update billing section
        if (response.update_section) {

            $('#checkout-' + response.update_section.name + '-load').html(response.update_section.html);
            $("#addaddressbutoonid").trigger('click');
            $(".new-shipping-address").toggle();
        }
        // Set newly created address 
        if (response.newAddressId > 0) {

            Shipping.newAddress(false);
            $('#shipping-address-select').val(response.newAddressId);
            // Add new value in billing address
            var newAddressText = $('#shipping-address-select option:selected').text();
            $('#billing-address-select').find('option[value=""]').remove();
            $('#billing-address-select').append($('<option></option>').val(response.newAddressId).html(newAddressText));
            $("#billing-address-select").append(new Option("New Address", ""));
            Shipping.save(); // newly added address load into shipping section.
            $('#shipping-addresses-form').find('.new-address-next-step-button').trigger('click');
        }
    },
    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
        // Set section According PickUpPoint
        if ($("#PickupPointsModel_PickupInStore").prop('checked') == true) {
            $('#pickup-points-form').show();
            $('#shipping-addresses-form').hide();
            $('#checkout-step-shipping-method').hide(); // hide shipping method when pickup point is selected
            // We change order total because it is actually done after shippingmethod.save(). 
            //but pickup point is checked so shipping method will not called.
            OPCOrderTotalsChange();
        }
        else {

            $('#pickup-points-form').hide();
            $('#shipping-addresses-form').show();
            if (window.CommonDefaults.shippingRequired) {
                if ($('#billing-address-select').length != 0) {
                    ShippingMethod.save();
                }
                $('#checkout-step-shipping-method').show();
            }
            else {
                $('#checkout-step-shipping-method').hide();
            }
        }
    },

    nextStep: function (response) {
        if (response.error) {
            if ((typeof response.message) == 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }
            $('.confirm-order-next-step-button').hide();
            displayAjaxLoading(false);
            return false;
        }
        Checkout.setStepResponse(response);
    },
    addressChange: function (data) {

        if (data.value) {

            displayAjaxLoading(true);
            this.save();
        }
    }
};



var ShippingMethod = {
    form: false,
    saveUrl: false,

    init: function (form, saveUrl) {
        this.form = form;
        this.saveUrl = saveUrl;
    },

    validate: function () {
        var methods = document.getElementsByName('shippingoption');
        if (methods.length == 0) {
            //$('.confirm-order-next-step-button').hide();
            displayAjaxLoading(false);
            //alert('Your order cannot be completed at this time as there is no shipping methods available for it. Please make necessary changes in your shipping address.');
            return false;
        }

        for (var i = 0; i < methods.length; i++) {
            if (methods[i].checked) {
                return true;
            }
        }
        alert('Please specify shipping method.');
        return false;
    },

    save: function () {
        //var methods = document.getElementsByName('paymentmethod');
        //if (methods.length === 0) {
        //    return false;
        //}
        //for (var i = 0; i < methods.length; i++) {
        //    if (methods[i].checked) {

        //        $('#' + methods[i].id).trigger('click')
        //    }
        //}
        if (Checkout.loadWaiting != false) return;

        if (this.validate()) {
            Checkout.setLoadWaiting('shipping-method');

            $.ajax({
                cache: false,
                async: true,
                url: this.saveUrl,
                data: $(this.form).serialize(),
                type: 'post',
                success: this.nextStep,
                complete: this.resetLoadWaiting,
                error: Checkout.ajaxFailure
            });
        }
    },

    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
        PaymentMethod.save();
    },

    nextStep: function (response) {
        if (response.error) {
            if ((typeof response.message) == 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }
            $('.confirm-order-next-step-button').hide();
            displayAjaxLoading(false);
            return false;
        }

        Checkout.setStepResponse(response);

    }
};



var PaymentMethod = {
    form: false,
    saveUrl: false,

    init: function (form, saveUrl) {
        this.form = form;
        this.saveUrl = saveUrl;
    },

    toggleUseRewardPoints: function (useRewardPointsInput) {
        if (useRewardPointsInput.checked) {
            $('#payment-method-block').hide();
        }
        else {
            $('#payment-method-block').show();
        }
        this.save();
    },

    validate: function () {
        var methods = document.getElementsByName('paymentmethod');
        if (methods.length == 0) {
            $('.confirm-order-next-step-button').hide();
            displayAjaxLoading(false);
            alert('Your order cannot be completed at this time as there is no payment methods available for it.');
            return false;
        }

        for (var i = 0; i < methods.length; i++) {
            if (methods[i].checked) {
                return true;
            }
        }
        alert('Please specify payment method.');
        return false;
    },

    save: function () {
        if (Checkout.loadWaiting != false) return;

        if (this.validate()) {
            Checkout.setLoadWaiting('payment-method');
            $.ajax({
                cache: false,
                async: true,
                url: this.saveUrl,
                data: $(this.form).serialize(),
                type: 'post',
                success: this.nextStep,
                complete: this.resetLoadWaiting,
                error: Checkout.ajaxFailure
            });
        }
    },

    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
        OPCOrderTotalsChange();
    },

    nextStep: function (response) {
        if (response.error) {
            if ((typeof response.message) == 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }
            $('.confirm-order-next-step-button').hide();
            displayAjaxLoading(false);
            return false;
        }
        $('.confirm-order-next-step-button').show();
        Checkout.setStepResponse(response);


    }
};



var PaymentInfo = {
    form: false,
    saveUrl: false,

    init: function (form, saveUrl) {
        this.form = form;
        this.saveUrl = saveUrl;
    },

    save: function () {
        if (Checkout.loadWaiting != false) return;
        displayAjaxLoading(true);
        Checkout.setLoadWaiting('payment-info');
        $.ajax({
            cache: false,
            async: true, // do not remove this. otherwise confirm order not having payment info.
            url: this.saveUrl,
            data: $(this.form).serialize(),
            type: 'post',
            success: this.nextStep,
            complete: this.resetLoadWaiting,
            error: Checkout.ajaxFailure
        });
    },

    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
        ConfirmOrder.save();
    },

    nextStep: function (response) {
        if (response.error) {
            if ((typeof response.message) == 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }
            displayAjaxLoading(false);
            return false;
        }

        Checkout.setStepResponse(response);
    }
};



var ConfirmOrder = {
    form: false,
    saveUrl: false,
    isSuccess: false,

    init: function (saveUrl, successUrl) {
        this.saveUrl = saveUrl;
        this.successUrl = successUrl;
    },

    save: function () {
        if (Checkout.loadWaiting != false) return;

        //terms of service
        var termOfServiceOk = true;
        if ($('#termsofservice').length > 0) {
            //terms of service element exists
            if (!$('#termsofservice').is(':checked')) {
                $("#terms-of-service-warning-box").dialog();
                termOfServiceOk = false;
            } else {
                termOfServiceOk = true;
            }
        }
        if (termOfServiceOk) {
            Checkout.setLoadWaiting('confirm-order');
            $.ajax({
                cache: false,
                url: this.saveUrl,
                type: 'post',
                success: this.nextStep,
                complete: this.resetLoadWaiting,
                error: Checkout.ajaxFailure
            });
        } else {
            displayAjaxLoading(false);
            return false;
        }
    },

    resetLoadWaiting: function (transport) {
        Checkout.setLoadWaiting(false, ConfirmOrder.isSuccess);
        Checkout.setLoadWaiting(false);
    },

    nextStep: function (response) {
        if (response.error) {
            if ((typeof response.message) == 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }
            Checkout.setLoadWaiting(false);
            displayAjaxLoading(false);
            return false;
        }

        if (response.redirect) {
            ConfirmOrder.isSuccess = true;
            location.href = response.redirect;
            return;
        }
        if (response.success) {
            ConfirmOrder.isSuccess = true;
            window.location = ConfirmOrder.successUrl;
        }

        Checkout.setStepResponse(response);
    }
};



function PaymentMethodChange(e) {
    displayAjaxLoading(true);
    PaymentMethod.save();
}

function ShippingMethodChange(e) {
    displayAjaxLoading(true);
    ShippingMethod.save();
}

function OPCOrderTotalsChange() {
    $.ajax({
        cache: false,
        url: '/opccheckout/opcordertotals?isEditable=True',
        data: $('#checkout-step-checkoutAttributes-form').serialize(),
        type: 'post',
        success: function (data) {
            if (data.ordetotalssectionhtml) {
                $('.total-info').replaceWith(data.ordetotalssectionhtml);
            }
            displayAjaxLoading(false);
        },
        error: function (data) {
            displayAjaxLoading(false);
            alert(data);
        }
    });
}

function discountgiftcardcouponcode(url, extraParameters) {
    var formData = $('#co-discountgift-form').serialize();
    if (extraParameters) {
        formData += extraParameters;
    }
    $.ajax({
        cache: false,
        url: url,
        data: formData,
        type: 'post',
        success: function (data) {
            if (data.update_section.name && data.update_section.html) {
                $('.' + data.update_section.name).replaceWith(data.update_section.html);
            }
            OPCOrderTotalsChange();
            var form = $("#shopping-cart-form").serialize();
            updateShoppingCartItem(form);
        }
    });
}

function displayAjaxLoading(display) {
    var imageURL = $(window).width() >= 768 ? "../Plugins/XcellenceIt.UniversalOnePageCheckout/Content/Images/ajax_loader_large_opc.gif" : "../Plugins/XcellenceIt.UniversalOnePageCheckout/Content/Images/ajax_loader_small.gif";

    if (display) {
        $.blockUI({
            message: "<img src=\"" + imageURL + "\" alt=\"please wait...\" />",
            css: {
                border: 'none',
                backgroundColor: 'transparent'
            }
        });
    }
    else {
        $.unblockUI();
    }
};

