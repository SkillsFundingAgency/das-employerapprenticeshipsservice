var pageTitle = document.querySelector('h1.govuk-heading-xl').innerText;

// Form validation - dataLayer pushes
var errorSummary = document.querySelector('.govuk-error-summary');
if (errorSummary !== null) {
    var validationErrors = errorSummary.querySelectorAll('ul.govuk-error-summary__list li');
    var numberOfErrors = validationErrors.length;
    var validationMessage = 'Form validation';
    var dataLayerObj;
    if (numberOfErrors === 1) {
        validationMessage = validationErrors[0].innerText
    }
    dataLayerObj = {
        event: 'form submission error',
        page: pageTitle,
        message: validationMessage
    }
    window.dataLayer.push(dataLayerObj)
}

// Radio button selection - dataLayer pushes
var radioWrapper = document.querySelector('.govuk-radios');
if (radioWrapper !== null) {
    var radios = radioWrapper.querySelectorAll('input[type=radio]');
    var labelText;
    var dataLayerObj;
    nodeListForEach(radios, function(radio) {
        radio.addEventListener('change', function() {
            labelText = this.nextElementSibling.innerText;
            dataLayerObj = {
                event: 'radio button selected',
                page: pageTitle,
                radio: labelText
            }
            window.dataLayer.push(dataLayerObj)
        })
    })
}

function nodeListForEach(nodes, callback) {
    if (window.NodeList.prototype.forEach) {
        return nodes.forEach(callback)
    }
    for (var i = 0; i < nodes.length; i++) {
        callback.call(window, nodes[i], i, nodes);
    }
} 