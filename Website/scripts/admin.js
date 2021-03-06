﻿(function ($) {
    
    var postId, isNew,
        txtTitle, txtExcerpt, txtContent, txtMessage, chkPublish,
        btnNew, btnEdit, btnDelete, btnSave, btnCancel, blogPath, editControls, editPreview,

    editPost = function () {
        txtTitle.attr('contentEditable', true);
        txtExcerpt.attr('contentEditable', true);
        txtExcerpt.css({ minHeight: "100px" });
        txtExcerpt.parent().css('display', 'block');
        txtContent.focus();

        btnNew.attr("disabled", true);
        btnEdit.attr("disabled", true);
        btnSave.removeAttr("disabled");
        btnCancel.removeAttr("disabled");
        chkPublish.removeAttr("disabled");

        showCategoriesForEditing();

        $("#tools").fadeIn().css("display", "inline-block");
    },
    cancelEdit = function () {
        if (isNew) {
            if (confirm("Do you want to leave this page?")) {
                history.back();
            }
        } else
        {
            window.location = window.location.href.split(/\?|#/)[0];
        }
    },
    savePost = function (e) {
        $.post(blogPath + "/post.ashx?mode=save", {
            id: postId,
            isPublished: chkPublish[0].checked,
            title: txtTitle.text().trim(),
            excerpt: txtExcerpt.text().trim(),
            content: txtContent.text().trim(),
            categories: getPostCategories(),
            pubDate: $("#pubDate").val(),
            __RequestVerificationToken: document.querySelector("input[name=__RequestVerificationToken]").getAttribute("value")
        })
          .success(function (data) {
              if (isNew) {
                  location.href = data;
                  return;
              }

              showMessage(true, "The post was saved successfully");
              cancelEdit(e);
          })
          .fail(function (data) {
              if (data.status === 409) {
                  showMessage(false, "The title is already in use");
              } else {
                  showMessage(false, "Something bad happened. Server reported " + data.status + " " + data.statusText);
              }
          });
    },
    deletePost = function () {
        if (confirm("Are you sure you want to delete this post?")) {
            $.post(blogPath + "/post.ashx?mode=delete", { id: postId, __RequestVerificationToken: document.querySelector("input[name=__RequestVerificationToken]").getAttribute("value") })
                .success(function () { location.href = blogPath+"/"; })
                .fail(function () { showMessage(false, "Something went wrong. Please try again"); });
        }
    },
    showMessage = function (success, message) {
        var className = success ? "alert-success" : "alert-error";
        txtMessage.addClass(className);
        txtMessage.text(message);
        txtMessage.parent().fadeIn();

        setTimeout(function () {
            txtMessage.parent().fadeOut("slow", function () {
                txtMessage.removeClass(className);
            });
        }, 4000);
    },
    getPostCategories = function () {
        var categories = '';

        if ($("#txtCategories").length > 0) {
            categories = $("#txtCategories").val();
        } else {
            $("ul.categories li a").each(function (index, item) {
                if (categories.length > 0) {
                    categories += ",";
                }
                categories += $(item).html();
            });
        }
        return categories;
    },
    showCategoriesForEditing = function () {
        var firstItemPassed = false;
        var categoriesString = getPostCategories();
        $("ul.categories li").each(function (index, item) {
            if (!firstItemPassed) {
                firstItemPassed = true;
            } else {
                $(item).remove();
            }
        });
        $("ul.categories").append("<li><input id='txtCategories' class='form-control' /></li>");
        $("#txtCategories").val(categoriesString);
    },
    showCategoriesForDisplay = function () {
        if ($("#txtCategories").length > 0) {
            var categoriesArray = $("#txtCategories").val().split(',');
            $("#txtCategories").parent().remove();

            $.each(categoriesArray, function (index, category) {
                $("ul.categories").append(' <li itemprop="articleSection" title="' + category + '"> <a href="'+blogPath+'/category/' + encodeURIComponent(category.toLowerCase()) + '">' + category + '</a> </li> ');
            });
        }
    };

    postId = $("[itemprop~='blogPost']").attr("data-id");

    txtTitle = $("[itemprop~='blogPost'] [itemprop~='name']");
    txtExcerpt = $("[itemprop~='description']");
    txtContent = $("[itemprop~='articleBody'] > textarea");
    txtMessage = $("#admin .alert");
    
    btnNew = $("#btnNew");
    btnEdit = $("#btnEdit");
    btnDelete = $("#btnDelete").bind("click", deletePost);
    btnSave = $("#btnSave").bind("click", savePost);
    btnCancel = $("#btnCancel").bind("click", cancelEdit);
    chkPublish = $("#ispublished").find("input[type=checkbox]");
    blogPath = $("#admin").data("blogPath");

    var commonmark = window.commonmark;
    var reader = new commonmark.Parser();
    var writer = new commonmark.HtmlRenderer({ sourcepos: true });

    editControls = $("#editBox, #editPreview");
    editPreview = $("#editPreview");
    $(".source").on("click", function() {
        editControls.toggleClass("showPreview");
        if (editControls.hasClass("showPreview")) {
            var parsed = reader.parse(txtContent.text());
            var result = writer.render(parsed);
            editPreview.html(result);
        }
    });

    $("#pubDate").datetimepicker({
        format: "Y-m-d H:i"
    });

    isNew = location.pathname.replace(/\//g, "") === blogPath.replace(/\//g, "") + "postnew";
    
    if (isNew) {
        editPost();
        $("#ispublished").fadeIn();
        chkPublish[0].checked = false;
    } else if (txtTitle !== null && txtTitle.length === 1 && location.pathname.length > 1) {
        if (location.search.indexOf("mode=edit") != -1)
            editPost();

        btnEdit.removeAttr("disabled");
        btnDelete.removeAttr("disabled");
        $("#ispublished").css({ "display": "inline" });
    }

})(jQuery);
