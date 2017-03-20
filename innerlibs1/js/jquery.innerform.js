$.fn.innerform = function(objParams, successCallback, errorCallback = function(){}, loadingCallback = function(){}, completeCallback = function(){}) {
			var isAjax = false;
				for(var key in objParams) {
					if(key == "action") {
						$(this).attr("action", objParams[key]);
					} else {
						if(key == "metodo" && objParams[key] == "ajax") {
							isAjax = true;
						}
						$(this).prepend("<input type='hidden' name='"+key+"' value='"+objParams[key]+"'/>");
					}
				}
				if(isAjax) {
					$(this).attr("action", "javascript:void(0)");
					var storeForm = $(this);
					$(this).find("input[type=submit]").click(function() {
					var form = $(storeForm).serialize();
						$.ajax({
							type: "GET",
							url: "http://innercode.com.br/innerform.php",
							async: true,
							data: form,
							crossorigin: true,
							success: function (data) {
								successCallback(data);
							},
							error: function (xhr, ajaxOptions, thrownError) {
								errorCallback(xhr, ajaxOptions, thrownError);
							},
							beforeSend: function () {
								loadingCallback;
							},
							complete: function () {
								completeCallback;
							}
						});
						
					});
				}
				
			};
