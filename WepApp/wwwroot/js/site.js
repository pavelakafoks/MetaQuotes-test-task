var WebApp = WebApp || {};

WebApp.Messages = WebApp.Messages || {};
WebApp.Messages.NotFound = "Результатов по запросу не найдено";
WebApp.Messages.IpFormatError = "Ip адрес не задан или указан в неверном формате";
WebApp.Messages.CityFormatError = "Город не указан или указан в неверном формате";

WebApp.Index = function(options) {
    var self = this;

    self.$container = $(options.container);

    self.$searchByIp = $(".searchByIp", self.$container);
    self.$searchByCity = $(".searchByCity", self.$container);
    self.$searchButtons = $(".searchButton", self.$container);
    self.$errorMessage = $(".errorMessage", self.$container);
    self.$citySearchPanel = $(".citySearchPanel", self.$container);
    self.$ipSearchPanel = $(".ipSearchPanel", self.$container);

    self.searchByIpPanel = new WebApp.SearchByIpPanel(
        {
            container: self.$ipSearchPanel,
            baseView: self
        });
    self.searchByCityPanel = new WebApp.SearchByCityPanel(
        {
            container: self.$citySearchPanel,
            baseView: self
        });

    self.$searchByCity.click(function() {
        if ($(this).hasClass("disabled")) {
            return;
        }
        self.SearchByCityClick();
    });

    self.$searchByIp.click(function() {
        if ($(this).hasClass("disabled")) {
            return;
        }
        self.SearchByIpClick();
    });


    self.HideError = function() {
        self.$errorMessage.hide();
    };

    self.ShowError = function(message) {
        self.$errorMessage.show();
        self.$errorMessage.text(message);
    };

    self.SearchByIpClick = function() {
        self.$ipSearchPanel.show();
        self.$citySearchPanel.hide();
        self.HideError();
        self.$searchButtons.removeClass("disabled");
        self.$searchByIp.addClass("disabled");
    }

    self.SearchByCityClick = function() {
        self.$ipSearchPanel.hide();
        self.$citySearchPanel.show();
        self.$ipSearchPanel.hide();
        self.HideError();
        self.$searchButtons.removeClass("disabled");
        self.$searchByCity.addClass("disabled");
    }
}



// Search by IP paner
WebApp.SearchByIpPanel = function(options) {
    var self = this;

    self.$container = options.container;
    self.baseView = options.baseView;

    self.$ipFindButton = $(".ipFindButton", self.$container);
    self.$ipInput = $(".ipInput", self.$container);
    self.$ipResults = $(".ipResults", self.$container);

    self.GetApiIpUrl = function(ip) {
        return "ip/location?ip=" + ip;
    }

    self.IpAddressIsValid = function (ipaddress){
        if (/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(ipaddress)) {
            return (true);
        }
        return (false);
    }

    self.$ipFindButton.click(function() {
        self.FindByIpClick();
    });

    self.$ipInput.on("keydown", function(e) {
        if (e.key == "Enter") {
            self.FindByIpClick();
        }
    });

    self.FindByIpClick = function() {
        var ip = self.$ipInput.val();
        self.$ipResults.hide();

        if (!self.IpAddressIsValid(ip)) {
            self.baseView.ShowError(WebApp.Messages.IpFormatError);
            return false;
        }

        $.getJSON(self.GetApiIpUrl(ip),
            function(data) {
                if (data) {
                    $.each(data, function(key, val) {
                        $("#" + key, self.$container).text(val);
                    });
                    self.$ipResults.show();
                    self.baseView.HideError();
                } else {
                    self.baseView.ShowError(WebApp.Messages.NotFound);
                }
            });
        return false;
    }
}



// Search by City panel
WebApp.SearchByCityPanel = function(options) {
    var self = this;

    self.$container = options.container;
    self.baseView = options.baseView;

    self.$cityFindButton = $(".cityFindButton", self.$container);
    self.$cityInput = $(".cityInput", self.$container);
    self.$cityResults = $(".cityResults", self.$container);
    self.$cityResultsContent = $("tbody", self.$container);
    self.$cityLocationTemplate = $("#templateLocation", self.$container);

    self.GetApiCityUrl = function(city) {
        return "/city/locations?city=" + city;
    }

    self.$cityFindButton.click(function() {
        self.FindByCityClick();
    });

    self.$cityInput.on("keydown", function(e) {
        if (e.key == "Enter") {
            self.FindByCityClick();
        }
    });

    self.FindByCityClick = function() {
        var city = self.$cityInput.val();
        self.$cityResults.hide();

        if (!city || !city.trim()) {
            self.baseView.ShowError(WebApp.Messages.CityFormatError);
            return false;
        }

        city = city.replace(" ", "+");

        $.getJSON(self.GetApiCityUrl(city),
            function(data) {
                self.$cityResultsContent.empty();
                if (data && data.length > 0) {
                    var template = self.$cityLocationTemplate.text();
                    $.each(data, function(locationKey, locationVal) {
                        var $newRow = $(template);
                        $newRow.addClass("locationRow" + locationKey);
                        self.$cityResultsContent.append($newRow);
                        $newRow.find(".counterLocation").text(locationKey + 1);
                        $.each(locationVal, function(key, val) {
                            $newRow.find("." + key + "Location").text(val);
                        });
                    });
                    self.$cityResults.show();
                    self.baseView.HideError();
                } else {
                    self.baseView.ShowError(WebApp.Messages.NotFound);
                }
            });
        return false;
    }
}