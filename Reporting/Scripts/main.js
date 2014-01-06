var Tax = function (){ };
$(function() {

    // Bootstrap tabs
    $(document).on('click', 'ul.nav-tabs a', function (e) {
        e.preventDefault();
        $(this).tab('show');
    });

    $(document).on('change', '.tax-form .country', function() {
        var country = $(this).val();
        $.get('/api/states', { 'country': country }, function(data) {
            $('.state').html('');
            $.each(data, function(i, state) {
                var opt = '<option value="' + state.Abbr + '">' + state.State + '</option>';
                $('.state').append(opt);
            });
        }, 'json');
    });

    $(document).on('click', '.tax-form button', function (e) {
        e.preventDefault();
        var tax = new Tax();
        tax.Country = $('.tax-form .country').val();
        tax.State = $('.tax-form .state').val();
        tax.Percent = $('.tax-form .tax').val();
        tax.add();
    });
    
    $(document).on('click', '.tax-table button', function (e) {
        e.preventDefault();

        $(this).closest('tr').remove();
        if($('.tax-table tbody tr').get().length === 0) {
            var html = '<tr class="empty"><td colspan="4" class="muted align-center">No tax records</td></tr>';
            $('.tax-table tbody').append(html);
        }
    });

    $(document).on('click', '.btn-generate', function () {
        var tax_arr = [];
        $.each($('.tax-table tbody tr'), function(i, row) {
            var tax = new Tax();
            tax.Country = $(row).find('td:nth-child(1)').text();
            tax.State = $(row).find('td:nth-child(2)').text();
            tax.Percent = $(row).find('td:nth-child(3)').text();
            tax_arr.push(tax);
        });
        $('.tax-holder').val(JSON.stringify(tax_arr));
    });
    $(".datePicker").datepicker();
    $(".piesdatepicker").datepicker();
    $('#allpiesparts').on('click', function (e) {
        e.preventDefault();
        $('.piesdatepicker').attr('value', '');
        $('.piesreportform').submit();
    });

    $("#example").popover({ trigger: 'click',  placement:'left' });
});

Tax.prototype = {
    Country: '',
    State: '',
    Percent: 0,
    add: function () {
        if(this.Country.length == 0 || this.State.length == 0 || this.Percent == 0) {
            $('.alert').remove();
            var html = '<div class="alert alert-error">';
            html += '<button type="button" class="close" data-dismiss="alert">x</button>';
            html += '<strong>Oh snap!</strong> You missed a few fields.';
            html += '</div>';
            $('.tax-form').before(html);
            return;
        }
        $('.tax-table tr.empty').remove();
        var row = '<tr>';
        row += '<td>' + this.Country + '</td>';
        row += '<td>' + this.State + '</td>';
        row += '<td>' + this.Percent + '</td>';
        row += '<td class="align-center"><button class="btn btn-mini btn-danger"><i class="icon-minus icon-white"></i></button></td>';
        row += '</tr>';
        $('.tax-table tbody').append(row);
    }
};

