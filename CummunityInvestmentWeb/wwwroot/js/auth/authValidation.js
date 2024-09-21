//$(document).ready(function () {
//    //--- Email validation
//    const email = $('#email');
//    const email_feedback = $('#email-feedback');
//    email.on('input', () => {
//        let regex_email = /[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/;
//        if (regex_email.test(email.val())) {
//            email_err = false;
//            for_valid(email, email_feedback);
//        } else {
//            email_err = true;
//            let err_msg = "Please enter valid Email address!";
//            for_invalid(email, email_feedback, err_msg);
//        }
//    })

//    //--- Helper functions
//    function for_invalid(element, err_msg_element, err_msg) {
//        err_msg_element.text(err_msg);
//        element.addClass("is-invalid");
//    }

//    function for_valid(element, err_msg_element) {
//        element.removeClass("is-invalid");
//    }
//});