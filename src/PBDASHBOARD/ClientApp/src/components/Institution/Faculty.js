import Chart from 'react-apexcharts'
import React, {Component} from 'react';
import Autocomplete from "@material-ui/lab/Autocomplete";
import Checkbox from "@material-ui/core/Checkbox";
import TextField from "@material-ui/core/TextField";
import CheckBoxOutlineBlankIcon from '@material-ui/icons/CheckBoxOutlineBlank';
import CheckBoxIcon from '@material-ui/icons/CheckBox';
import axios from "axios";

const icon = <CheckBoxOutlineBlankIcon fontSize="small"/>;
const checkedIcon = <CheckBoxIcon fontSize="small"/>;
let name = [];// date/year
let namefc = [];
let amountOfPubliction = [];
let category = []; //Faculty names
let year1 = '';
let year2 = '';
let year3 = '';
let amount1 = [];
let amountf1 = [];
let amount2 = [];
let amount3 = [];
let yearf = '';
let yearf1 = '';
const amountf = [];
let demosCategry = [];
let categoryfc = []; //Faculty names


let year12 = '';
let year22 = '';
let year32 = '';
let amount12 = [];
let amount22 = [];


export class Faculty extends Component {
    constructor(props) {
        super(props);
        this.state = {
            amountOfResearches: [],
            loading: true,
            series: [{

                name: yearf,
                data: amountf
            }, {
                name: yearf1,
                data: amountf1
            },],

            options: {
                chart: {
                    type: 'bar',
                    height: 350,
                    width: '350%'
                },

                colors: ["#F3B415", "#F27036", "#663F59", "#6A6E94", "#4E88B4", "#00A7C6", "#18D8D8", '#A9D794',
                    '#46AF78', '#A93F55'
                ],
                plotOptions: {
                    bar: {
                        horizontal: false,
                        columnWidth: '55%',
                        endingShape: 'rounded',


                    },
                },
                dataLabels: {
                    enabled: false
                },
                stroke: {
                    show: true,
                    width: 2,
                    colors: ['transparent']
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
                    categories: categoryfc,
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
                    }
                }
            },

        };
        this.HandleChangers = this.HandleChangers.bind(this)

    }


    async componentDidMount() {
        await this.Faculties();
        await this.Rfaculties();

    }

    //  institutes = new institueWithYears(this.props);


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
                    <Autocomplete /*is a normal text input enhanced by a panel of suggested options.*/
                        multiple  /*this helps us have multiple results in the search bar or we can search for multiple things at once*/
                        id="checkboxes-tags-demo"
                        options={this.state.amountOfResearches}

                        renderOption={(option, {selected}) => (


                            <React.Fragment>
                                <Checkbox
                                    icon={icon}
                                    checkedIcon={checkedIcon}
                                    style={{marginRight: 8}}
                                    checked={selected}
                                    value={option}
                                    onChange={this.HandleChangers}
                                />{option}
                            </React.Fragment>)}

                        style={{width: 500}}
                        renderInput={(params) => (
                            <TextField {...params} variant="outlined"
                                       label="Checkboxes"
                                       placeholder="Favorites"/>)}/>
                </div>
                <Chart options={this.state.options} series={this.state.series} type="bar" width={590} height={450}
                       style={{
                           display: 'flex',
                           alignItems: 'center',
                           justifyContent: 'center',
                           marginCenter: 'center',
                           marginInline: 'center'

                       }}/>
            </div>
        );

    }


    componentDidUpdate(prevProps, prevState, snapshot) {


    }

    //This function is meant to send post request and return response from controller. 
    // You will get institutes and how amount of puplications 


    async Rfaculties() {

        const response = await fetch(window.location.origin+'/Data/GetListOfAllFaculties');/*i can put the url to the API here*/
        const Faculty = await response.json();

        this.setState({amountOfResearches: Faculty, loading: false});
    }


    async Faculties() {
        if (namefc.length === 0) {
            const responseGraph = await fetch(window.location.origin+'/Data/GroupByFaculty');
            const GFaculty = await responseGraph.json();
            for (let item in GFaculty) {
                for (const [key, value] of Object.entries(GFaculty[item])) {
                    if (!categoryfc.includes(item)) {
                        categoryfc.push(item);
                    }
                    if (!namefc.includes(key)) {
                        namefc.push(key);


                    } else
                        amountOfPubliction.push(value);


                    yearf = namefc[3];
                    yearf1 = namefc[4];


                    switch (key) {
                        case '2020':
                            amountf.push(value);
                            break;

                        case '2021':
                            amountf1.push(value);
                            break;


                    }


                }
            }


        }
        console.log(amount1)


    }


    async HandleChangers(event) {
        categoryfc.length = 0;
        yearf="";
        amountf.length = 0;
        yearf1 = "";
        amountf1.length = 0;
        let dl = this.state;
        const dataseries = [{
            name: year12,
            data: amount12
        }, {
            name: year22,
            data: amount22
        }];


        if (event.target.checked === true) {
            category.length = 0; //Faculty names
            year1 = '';
            year2 = '';
            amount1.length = 0;
            amount2.length = 0;
            categoryfc.length = 0;
            let requestData = JSON.stringify({
                name: event.target.value
            });
            await axios({
                method: "Post",
                url: "https://localhost:5001/Data/InstituteAndPublication",
                data: requestData,
                headers: {'Content-Type': 'application/json'},
            })
                .then(function (response) {
                    let data2 = response.data;
                    for (let item in data2) {
                        for (const [key, value] of Object.entries(data2[item])) {
                            if (!categoryfc.includes(item)) {
                                categoryfc.push(item);
                            }
                            if (!name.includes(key)) {
                                name.push(key);
                            }
                            year12 = name[3];
                            year22 = name[4];
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
                })
                .catch(function (response) {
                    //handle error
                });


            await this.setState({series: dataseries})


        } else if (event.target.checked === false) {
            namefc.length = 0;
            categoryfc.length = 0;
            await this.componentDidMount();
            const dat = [{
                name: yearf,
                data: amountf
            }, {
                name: yearf1,
                data: amountf1
            }];

            dl.options.xaxis = categoryfc;
            this.setState({options: dl});
            await this.setState({series: dat})
        }


    }


}