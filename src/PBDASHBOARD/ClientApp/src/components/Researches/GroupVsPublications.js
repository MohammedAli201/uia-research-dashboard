import Chart from 'react-apexcharts'
import React, {Component} from 'react';

// amountOfPublication: will hold the number of pub
let countPublication = [];
// NameofGroups: will hold names of grps
let nameofGroups = [];
let amountClicks = 0
let n = [];
let m = [];

let forSwitchStateValue = [];


export class GroupVsPublications extends Component {
    constructor(props) {
        super(props);
        this.state = {
            amounts: [],

            msg: 'Next top 10',
            next:0,

            series: [{
                name: '',
                data: countPublication,

            }],

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
                        distributed: true

                    },
                    colors: ["#F3B415", "#h27036434", "#663F59", "#6A6E94", "#4E88B4", "#00A7C6", "#18D8D8", '#A9D794',"#C3B425","#A3B499",
                        '#46AF78', '#A93F55',"#F3B433","#h2222"
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
                    categories: nameofGroups,
                    labels: {
                        show: false,
                        rotate: -45,
                        rotateAlways: false,
                        hideOverlappingLabels: true,
                        showDuplicates: false,
                        trim: false,
                        minHeight: undefined,
                        maxHeight: 120,
                        style: {
                            colors: [],
                            fontSize: '12px',
                            fontFamily: 'Helvetica, Arial, sans-serif',
                            fontWeight: 400,
                            cssClass: 'apexcharts-xaxis-label',
                        }
                    }
                },
                yaxis: {
                    title: {
                        text: ' (10 top Publication)'
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

        this.onClickNext = this.onClickNext.bind(this);

    }

    async componentDidMount() {
        await this.pubGraph();
    }


    //  institutes = new institueWithYears(this.props);
    render() {


        return (

            <div>
                <Chart options={this.state.options} series={this.state.series} type="bar" width={850} height={450}
                       style={{
                           display: 'flex',
                           alignItems: 'center',
                           justifyContent: 'center',
                           marginCenter: 'center',
                           marginInline: 'center'
                       }}/>

                <div className="container-fluid" style={{marginLeft: '50%'}} onClick={this.onClickNext}>
                    <button type="submit">{this.state.next} </button>
                </div>

            </div>


        );


    }

    async onClickNext() {
        let dl= this.state;

        nameofGroups.length = 0;
        countPublication.length = 0;

        amountClicks++;
        if (amountClicks>6){
            amountClicks=0;


        }
        let final = [];






        switch (amountClicks) {
            case 0:
                final.length=0;

                final = forSwitchStateValue.slice(0, 10);
                for (let item in final) {
                    nameofGroups.push(final[item][0]);
                    countPublication.push(final[item][1]);
                }
                break;
            case 1:
                final.length=0;

                final = forSwitchStateValue.slice(10, 20);
                for (let item in final) {
                    nameofGroups.push(final[item][0]);
                    countPublication.push(final[item][1]);
                }
                break;
            case 2:
                final.length=0;
                final = forSwitchStateValue.slice(20, 30);
                for (let item in final) {
                    nameofGroups.push(final[item][0]);
                    countPublication.push(final[item][1]);
                }


                break;
            case 3:
                final.length=0;
                final = forSwitchStateValue.slice(30, 40);
                for (let item in final) {
                    nameofGroups.push(final[item][0]);
                    countPublication.push(final[item][1]);
                }


                break;
            case 4:
                final.length=0;
                final = forSwitchStateValue.slice(40, 50);
                for (let item in final) {
                    nameofGroups.push(final[item][0]);
                    countPublication.push(final[item][1]);
                }

                break;

            case 5:
                final.length=0;
                final = forSwitchStateValue.slice(50, 60);
                for (let item in final) {
                    nameofGroups.push(final[item][0]);
                    countPublication.push(final[item][1]);
                }

                break;
            case 6:
                final.length=0;
                final = forSwitchStateValue.slice(60, 70);
                for (let item in final) {
                    nameofGroups.push(final[item][0]);
                    countPublication.push(final[item][1]);
                }

                break;

        }


        dl.series.data=countPublication;
        dl.options=nameofGroups;

        dl.next=amountClicks;

        await this.setState({series:dl.series});
        await this.setState({options:dl.option})




    }


    async pubGraph() {
        if (countPublication.length === 0) {

            const responseGraph = await fetch('Data/GetGroupWithPublicationa');
            const GFaculty = await responseGraph.json();
            let sortable = [];
            for (let vehicle in GFaculty) {
                sortable.push([vehicle, GFaculty[vehicle]]);
                forSwitchStateValue.push([vehicle, GFaculty[vehicle]])

            }

            sortable.sort(function (a, b) {
                return a[1] - b[1];
            });
            forSwitchStateValue.sort(function (a, b) {
                return a[1] - b[1];
            });


            sortable.reverse();
            forSwitchStateValue.reverse();
            let final = sortable.slice(0, 10);

            for (let item in final) {
                nameofGroups.push(final[item][0]);
                countPublication.push(final[item][1]);
            }
        }


    }


}