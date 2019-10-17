(function (factory) {
   if (typeof define === "function" && define.amd) {
      define(["jquery", "../jquery.validate"], factory);
   } else if (typeof module === "object" && module.exports) {
      module.exports = factory(require("jquery"));
   } else {
      factory(jQuery);
   }
}(function ($) {
   $.extend($.validator.messages, {
      required: "กรุณาระบุ",
      remote: "กรุณาแก้ไขข้อมูลให้ถูกต้อง",
      email: "กรุณาระบุที่อยู่อีเมลที่ถูกต้อง",
      url: "กรุณาระบุ URL ที่ถูกต้อง",
      date: "กรุณาระบุวันที่ ที่ถูกต้อง",
      dateISO: "กรุณาระบุวันที่ ที่ถูกต้อง (ระบบ ISO).",
      number: "กรุณาระบุทศนิยมที่ถูกต้อง",
      digits: "กรุณาระบุเฉพาะตัวเลข",
      creditcard: "กรุณาระบุรหัสบัตรเครดิตที่ถูกต้อง",
      equalTo: "รหัสผ่านไม่ตรงกัน",
      extension: "กรุณาระบุค่าที่มีส่วนขยายที่ถูกต้อง",
      maxlength: $.validator.format("ความยาวต้องไม่เกิน {0} ตัวอักษร"),
      minlength: $.validator.format("ความยาวต้องเท่ากับ {0} ตัวอักษร"),
      rangelength: $.validator.format("ความยาวต้องมากกกว่าหรือเท่ากับ {0} ตัวอักษรและไม่เกิน {1} ตัวอักษร"),
      range: $.validator.format("ต้องมีค่าไม่น้อยกว่า {0} และไม่เกิน {1}"),
      max: $.validator.format("กรุณาระบุค่าน้อยกว่าหรือเท่ากับ {0}"),
      min: $.validator.format("กรุณาระบุค่ามากกว่าหรือเท่ากับ {0}")
   });
   return $;
}));

$(document).ready(function () {
   $('body').addClass('login-page');
  
   $("#form-register, #form-login, #form-profile, #form, #form-contact").validate({
      submitHandler: function (form) {
         form.submit();
      },
      errorClass: "warning",
      showErrors: function (errorMap, errorList) {
         if (errorList != null) {
            for (var i = 0; i < errorList.length; i++) {
               var error = errorList[i];
               if (error.method == 'required') {
                  error.message = error.message + error.element.placeholder;
               }
            }
            this.defaultShowErrors();
         }
      }
   });
});

function LetterContainUpper(str) {
   for (var i = 0; i < str.length; i++) {
      if (isNaN(str[i])) {
         if (str[i] == str[i].toUpperCase()) {
            return true;
         }
      }
   }
   return false;
}

function LetterContainLower(str) {
   for (var i = 0; i < str.length; i++) {
      if (isNaN(str[i])) {
         if (str[i] == str[i].toLowerCase()) {
            return true;
         }
      }
   }
   return false;
}

function LetterContainNumber(str) {
   for (var i = 0; i < str.length; i++) {
      if (!isNaN(str[i])) {
         return true;
      }
   }
   return false;
}

function validatepwd(pwd) {
   var msg = "";
   if (pwd.length < 8)
      return false;

   if (!LetterContainUpper(pwd))
      return false;

   if (!LetterContainNumber(pwd))
      return false;

   return true;
}

function pwdtoggle(id) {
   var x = document.getElementById(id);
   if (x.type === "password") {
      x.type = "text";
   } else {
      x.type = "password";
   }

}