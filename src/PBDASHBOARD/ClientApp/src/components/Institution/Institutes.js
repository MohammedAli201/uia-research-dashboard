import React, {Component} from 'react';


import CheckBoxOutlineBlankIcon from '@material-ui/icons/CheckBoxOutlineBlank';

import Chart from 'react-apexcharts';


const icon = <CheckBoxOutlineBlankIcon fontSize="small"/>;
let name = [];// date/year




let category2 = []; //Faculty names
let year12 = '';
let year22 = '';
let year32 = '';
let amount12 = [];
let amount22 = [];
let amount32 = [];




export class Institute extends Component {
    constructor(props) {
        super(props);
        this.state = {
            amountOfResearches: [],
            loading: true,
            series: [{
                name: year12,
                data: amount12
            }, {
                name: year22,
                data: amount22
            },],

            options: {
                chart: {
                    type: 'bar',
                    height: 800,
                    width: '350%'

                },
                plotOptions: {
                    bar: {
                        horizontal: false,
                        columnWidth: '55%',
                        endingShape: 'rounded',


                    },
                    colors: ["#F3B415", "#F27036", "#663F59", "#6A6E94", "#4E88B4", "#00A7C6", "#18D8D8", '#A9D794',
                        '#46AF78', '#A93F55'
                    ],
                },
                dataLabels: {
                    enabled: false
                },
                grid: {
                    padding: {
                        top: -10,
                        right: 0,
                        bottom: 70,
                        left: 100

                    }
                },
                stroke: {
                    show: true,
                    width: 10,
                    colors: ['transparent']
                },
                xaxis: {
                    categories: category2,

                },
                yaxis: {
                    title: {
                        text: ' (Publications)'
                    }

                },

                fill: {
                    opacity: 1
                },
                tooltip: {
                    y: {
                        formatter: function (val) {
                            return " " + val + " Publication"
                        }
                    },


                }
            },

        };

    }

    async componentDidMount() {
        await this.Clicker()
    }

    render() {

        return (
            <div style={{marginRight: '7%', marginTop: '60px'}}>
                <h1>Faculty</h1>
                <div style={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    marginCenter: 'center',
                    marginInline: 'center'

                }}>

                </div>
                <Chart options={this.state.options} series={this.state.series} type="bar" width={850} height={450}
                       style={{
                           display: 'flex',
                           alignItems: 'center',
                           justifyContent: 'flex-end',
                           marginCenter: 'center',
                           marginInline: 'center'

                       }}/>
            </div>
        );

    }


    async Clicker() {
        if (category2.length === 0) {
            let response = await fetch(window.location.origin+'/Data/listOfPubLicationWithInsititute');
            let dat = await response.json();
            console.log(dat);

            for (let item in dat) {
                for (const [key, value] of Object.entries(dat[item])) {
                    if (!category2.includes(item)) {
                        category2.push(item);
                    }
                    if (!name.includes(key)) {
                        name.push(key);
                    }

                    year12 = name[0];
                    year22 = name[1];


                    switch (key) {
                        case '2020':
                            amount12.push(value);
                            break;

                        case '2021':
                            amount22.push(value);
                            break;


                    }
                }
            }
        }


    }


}
