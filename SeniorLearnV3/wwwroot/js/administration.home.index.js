//admistration.home.index
$(() => {
    $('#extended-close').on('click', e => $('#extended-modal').modal('hide'));

    const showExtended = data => {
        $('#extended-modal').find('.modal-title').html(`Inactive Members: ${data.month}`);
        let el = $(`<div>
                        <table class="table table-striped">
                            <tr><td>Member</td><td>Future Enrolments</td></tr>
                        </table>
                    </div>`); 
        for (let m of data.members) {
            el.find('table').append(`<tr><td>${m.name}</td><td>${m.lessons}</td></tr>`);
        }
        $('#extended-modal').find('.modal-body').html(el.html());
        $('#extended-modal').modal('show');
    }

    fetch('/api/members/inactive-with-enrolments-report-data')
        .then(r => r.json())
        .then(j => {
        
            const chart = new Chart(document.getElementById('bar-chart'),{
                type: 'bar',
                data: {
                    datasets: [{
                        data: j.aggregate
                    }]
                },
                options: {
                    plugins: {
                        legend: {
                            display: false,
                        }
                    },
                    onClick: (c, i) => showExtended(j.extended.find(r => r.month == chart.data.datasets[0].data[i[0].index].x)),
                }
            })

        })
        .catch(e => console.error(e.message));  
})