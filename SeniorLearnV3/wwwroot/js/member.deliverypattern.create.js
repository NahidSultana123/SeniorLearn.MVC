//member.deliverypattern.create
$(() => {
    $('#PatternType').on('change', e => {
        $('.repeating').hide();
        $('.week-days').hide();

        if (e.target.value > 0) {
            $('.repeating').show();
            if (e.target.value == 1) {
                $('.week-days').hide();
            }
            else {
                $('.week-days').show();
            }
        }
    });
    $('#PatternType').trigger('change');


    $('#DeliveryModeId').on('change', e => {
        $('.delivery-location').hide();
        $('.delivery-url').hide();
        switch (e.target.value) {
            case '1': $('.delivery-location').show(); break;
            case '2': $('.delivery-url').show(); break;
            default: break;
        }
    });

    $('#DeliveryModeId').trigger('change');
    //$('#Initialize').on('change', e => $(e.target).prop('checked') ? $('.lesson-template').show() : $('.lesson-template').hide());
   // $('#Initialize').trigger('change');
});