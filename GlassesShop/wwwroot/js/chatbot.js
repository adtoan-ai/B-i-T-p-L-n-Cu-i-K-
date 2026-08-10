
(function () {
    var options = null;
    var answers = {};
    var currentStepIndex = 0;
    var started = false;

    var priceRanges = [
        { label: 'Dưới 500.000₫', min: null, max: 500000 },
        { label: '500.000₫ - 1.000.000₫', min: 500000, max: 1000000 },
        { label: '1.000.000₫ - 2.000.000₫', min: 1000000, max: 2000000 },
        { label: '2.000.000₫ - 5.000.000₫', min: 2000000, max: 5000000 },
        { label: 'Trên 5.000.000₫', min: 5000000, max: null }
    ];

    function steps() {
        return [
            {
                key: 'categoryId',
                question: 'Bạn muốn tìm <b>loại kính</b> nào?',
                options: function () {
                    return (options.categories || []).map(function (c) {
                        return { label: c.name, value: c.id };
                    });
                }
            },
            {
                key: 'style',
                question: 'Bạn thích <b>kiểu dáng</b> gọng kính nào?',
                options: function () {
                    return (options.styles || []).map(function (s) {
                        return { label: s, value: s };
                    });
                }
            },
            {
                key: 'brandId',
                question: 'Bạn có <b>thương hiệu</b> yêu thích không?',
                options: function () {
                    return (options.brands || []).map(function (b) {
                        return { label: b.name, value: b.id };
                    });
                }
            },
            {
                key: 'color',
                question: 'Bạn thích <b>màu sắc</b> nào?',
                options: function () {
                    return (options.colors || []).map(function (c) {
                        return { label: c, value: c };
                    });
                }
            },
            {
                key: 'priceRangeIndex',
                question: 'Bạn mong muốn <b>mức giá</b> khoảng bao nhiêu?',
                options: function () {
                    return priceRanges.map(function (r, i) {
                        return { label: r.label, value: i };
                    });
                }
            }
        ];
    }

    function escapeHtml(text) {
        return $('<div>').text(text == null ? '' : text).html();
    }

    function scrollToBottom() {
        var body = document.getElementById('chatbotBody');
        if (body) body.scrollTop = body.scrollHeight;
    }

    function addBotMessage(html) {
        $('#chatbotBody').append('<div class="chatbot-msg chatbot-msg-bot">' + html + '</div>');
        scrollToBottom();
    }

    function addUserMessage(text) {
        $('#chatbotBody').append('<div class="chatbot-msg chatbot-msg-user">' + escapeHtml(text) + '</div>');
        scrollToBottom();
    }

    function clearQuickReplies() {
        $('#chatbotBody .chatbot-quick-replies').remove();
    }

    function showQuickReplies(items) {
        clearQuickReplies();
        var html = '<div class="chatbot-quick-replies">';
        $.each(items, function (i, item) {
            html += '<button type="button" class="chatbot-quick-btn" data-index="' + i + '">' +
                escapeHtml(item.label) + '</button>';
        });
        html += '<button type="button" class="chatbot-quick-btn chatbot-skip-btn" data-skip="1">Bỏ qua</button>';
        html += '</div>';
        $('#chatbotBody').append(html);

        $('#chatbotBody .chatbot-quick-replies').data('items', items);
        scrollToBottom();
    }

    function resetConversation() {
        answers = {};
        currentStepIndex = 0;
        $('#chatbotBody').empty();
        addBotMessage('Chào bạn! Mình là trợ lý tư vấn chọn kính của cửa hàng. ' +
            'Để gợi ý đúng ý bạn nhất, mình xin hỏi vài câu nhé 😊');
        loadOptionsAndAsk();
    }

    function loadOptionsAndAsk() {
        
        $.getJSON('/ChatBot/GetOptions')
            .done(function (data) {
                options = data;
                askCurrentStep();
            })
            .fail(function () {
                addBotMessage('Xin lỗi, hiện không tải được dữ liệu tư vấn. Bạn vui lòng thử lại sau.');
            });
    }

    function askCurrentStep() {
        var stepList = steps();

        if (currentStepIndex >= stepList.length) {
            doSearch();
            return;
        }

        var step = stepList[currentStepIndex];
        var items = step.options();

        if (items.length === 0) {
          
            currentStepIndex++;
            askCurrentStep();
            return;
        }

        addBotMessage(step.question);
        showQuickReplies(items);
    }

    function handleAnswer(value, label) {
        var stepList = steps();
        var step = stepList[currentStepIndex];

        addUserMessage(label);
        clearQuickReplies();

        if (value !== null && value !== undefined) {
            answers[step.key] = value;
        }

        currentStepIndex++;
        askCurrentStep();
    }

    function buildFilterPayload() {
        var filter = {
            categoryId: answers.categoryId || null,
            style: answers.style || null,
            brandId: answers.brandId || null,
            color: answers.color || null,
            minPrice: null,
            maxPrice: null
        };

        if (answers.priceRangeIndex !== undefined && answers.priceRangeIndex !== null) {
            var range = priceRanges[answers.priceRangeIndex];
            filter.minPrice = range.min;
            filter.maxPrice = range.max;
        }

        return filter;
    }

    function doSearch() {
        addBotMessage('Cảm ơn bạn! Để mình tìm kính phù hợp nhất... <span class="chatbot-typing">⏳</span>');

        var filter = buildFilterPayload();

        $.ajax({
            url: '/ChatBot/Search',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(filter)
        }).done(function (res) {
            renderResults(res, filter);
        }).fail(function () {
            addBotMessage('Xin lỗi, có lỗi xảy ra khi tìm sản phẩm. Bạn thử lại giúp mình nhé.');
            showRestartQuickReply();
        });
    }

    function renderResults(res, filter) {
        if (!res.products || res.products.length === 0) {
            addBotMessage('Mình chưa tìm thấy sản phẩm nào khớp hết các tiêu chí bạn chọn. ' +
                'Bạn thử bắt đầu lại với ít tiêu chí hơn xem sao nhé!');
            showRestartQuickReply();
            return;
        }

        var html = '<div>Mình tìm thấy <b>' + res.totalCount + '</b> sản phẩm phù hợp, gợi ý cho bạn:</div>';
        addBotMessage(html);

        var cardsHtml = '<div class="chatbot-results">';
        $.each(res.products, function (i, p) {
            cardsHtml += '<a class="chatbot-product-card" href="/Product/Details/' + p.id + '" target="_blank">' +
                '<img src="' + p.image + '" onerror="this.onerror=null;this.src=\'/images/no-image.png\';" />' +
                '<div class="chatbot-product-info">' +
                '<div class="chatbot-product-name">' + escapeHtml(p.name) + '</div>' +
                '<div class="chatbot-product-meta">' + escapeHtml(p.brand) + (p.style ? ' • ' + escapeHtml(p.style) : '') + '</div>' +
                '<div class="chatbot-product-price">' + p.priceText + '</div>' +
                '</div></a>';
        });
        cardsHtml += '</div>';
        $('#chatbotBody').append(cardsHtml);
        scrollToBottom();

        var viewAllUrl = buildViewAllUrl(filter);
        addBotMessage('<a href="' + viewAllUrl + '" target="_blank" class="chatbot-viewall-link">' +
            'Xem tất cả kết quả trên trang sản phẩm <i class="bi bi-box-arrow-up-right ms-1"></i></a>');

        showRestartQuickReply();
    }

    function buildViewAllUrl(filter) {
        var params = [];
        if (filter.categoryId) params.push('categoryId=' + encodeURIComponent(filter.categoryId));
        if (filter.brandId) params.push('brandId=' + encodeURIComponent(filter.brandId));
        if (filter.style) params.push('style=' + encodeURIComponent(filter.style));
        if (filter.color) params.push('color=' + encodeURIComponent(filter.color));
        if (filter.minPrice) params.push('minPrice=' + encodeURIComponent(filter.minPrice));
        if (filter.maxPrice) params.push('maxPrice=' + encodeURIComponent(filter.maxPrice));
        return '/Product/Index' + (params.length > 0 ? '?' + params.join('&') : '');
    }

    function showRestartQuickReply() {
        var html = '<div class="chatbot-quick-replies">' +
            '<button type="button" class="chatbot-quick-btn chatbot-restart-inline-btn">' +
            '<i class="bi bi-arrow-counterclockwise me-1"></i>Tìm lại từ đầu</button>' +
            '</div>';
        $('#chatbotBody').append(html);
        scrollToBottom();
    }

    $(function () {
        $('#chatbotToggleBtn').on('click', function () {
            $('#chatbotWindow').toggleClass('d-none');
            if (!$('#chatbotWindow').hasClass('d-none') && !started) {
                started = true;
                resetConversation();
            }
        });

        $('#chatbotCloseBtn').on('click', function () {
            $('#chatbotWindow').addClass('d-none');
        });

        $('#chatbotRestartBtn').on('click', function () {
            resetConversation();
        });

        $(document).on('click', '.chatbot-restart-inline-btn', function () {
            resetConversation();
        });

        $(document).on('click', '.chatbot-quick-btn', function () {
            var $btn = $(this);

            if ($btn.data('skip')) {
                handleAnswer(null, 'Bỏ qua');
                return;
            }

            var index = $btn.data('index');
            var items = $btn.closest('.chatbot-quick-replies').data('items');
            var item = items[index];
            handleAnswer(item.value, item.label);
        });
    });
})();