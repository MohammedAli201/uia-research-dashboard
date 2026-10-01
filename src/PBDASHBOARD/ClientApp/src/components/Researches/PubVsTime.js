import Chart from 'react-apexcharts'
import React, {Component} from 'react';
import {AmpStoriesOutlined, AttachMoney} from "@material-ui/icons";

let name = [];// date/year

let amountOfPublication= [];



export class PubVsTime extends Component {
    constructor(props) {
        super(props);
        this.state = {

            series: [{
                name: "",
                data: amountOfPublication

            }],
            options: {
                chart: {
                    type: 'bar',
                    height: 850,
                    width: '350%'
                },
                plotOptions: {
                    bar: {
                        horizontal: false,
                        columnWidth: '65%',
                        endingShape: 'rounded'
                    },
                },
                dataLabels: {
                    enabled: false
                },
                stroke: {
                    show: true,
                    width: 7,
                    colors: ['#fff', '#f2f2f2']
                },
                grid: {
                    padding: {
                        top: -10,
                        right: 0,
                        bottom: 70,
                        left: 50

                    }
                },
                xaxis: {
                    categories: name,
                },
                yaxis: {
                    title: {
                        text: ' (Amount of Publication in UIA Since 2017 )'
                    }
                },
                fill: {
                    opacity: 1
                },
                tooltip: {
                    y: {
                        formatter: function (val) {
                            return " " + val + " Publication "
                        }
                    }
                }
            },

        };
    }


    async componentDidMount() {

        if (amountOfPublication.length === 0) {
            const responseGraph = await fetch('Data/publicationByDate');
            const GFaculty = await responseGraph.json();
            console.log("heiiiii1111111");

            for (let item in GFaculty) {
                console.log("heiiii2222222");

                amountOfPublication.push(GFaculty[item]);
                name.push(item);


            }
        }
    }




    render() {

        return (

            <Chart options={this.state.options} series={this.state.series} type="bar" width={550} height={400}
                   style={{
                       flex: 1,
                       display: 'flex',
                       alignItems: 'center',
                       justifyContent: 'center',
                       marginCenter: 'center',
                       marginInline: 'center'}}/>
        );

    }
}
