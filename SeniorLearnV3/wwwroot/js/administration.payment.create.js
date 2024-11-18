//administration.payment.create
$(() => {
    $('#MediaTypeId').on('change', e => {
        $('.cheque, .credit, .eft').hide();
        switch (e.target.value) {
            case '1': $('.cheque').show(); break;
            case '2': $('.credit').show(); break;
            case '3': $('.eft').show(); break;
        };
    });
    $('#MediaTypeId').trigger('change');
});