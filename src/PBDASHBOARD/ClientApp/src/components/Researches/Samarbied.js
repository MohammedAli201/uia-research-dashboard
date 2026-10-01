import React ,{Component} from "react";
import Chart from 'react-apexcharts'

const s=[];
const l=[];
export class Samarbied extends Component{
    constructor(props) {
        super(props);

        this.state = {

            series: s,
            options: {
                chart: {
                    width: 380,
                    type: 'donut',
                    dropShadow: {
                        enabled: true,
                        color: '#111',
                        top: -1,
                        left: 3,
                        blur: 3,
                        opacity: 0.2
                    }
                },
                stroke: {
                    width: 0,
                },
                grid: {
                    padding: {
                        top: -10,
                        right: 0,
                        bottom: 70,
                        left: 100

                    }
                },
                plotOptions: {
                    pie: {
                        donut: {
                            labels: {
                                show: true,
                                total: {
                                    showAlways: true,
                                    show: true
                                }
                            }
                        }
                    }
                },
                labels: l,
                dataLabels: {
                    dropShadow: {
                        blur: 3,
                        opacity: 0.8
                    }
                },
                fill: {
                    type: 'pattern',
                    opacity: 1,
                    pattern: {
                        enabled: true,
                        style: ['verticalLines', 'squares', 'horizontalLines', 'circles','slantedLines'],
                    },
                },
                states: {
                    hover: {
                        filter: 'none'
                    }
                },
                theme: {
                    palette: 'palette2'
                },
                title: {
                    text: "Favourite Movie Type"
                },
                responsive: [{
                    breakpoint: 480,
                    options: {
                        chart: {
                            width: 200
                        },
                        legend: {
                            position: 'bottom'
                        }
                    }
                }]
            },


        };
    
    }
   async componentDidMount() {
      await  this.cooperations()
        
    }
    async  cooperations(){
        let respon= await fetch('/Data/samarbied');
        let data = await respon.json();
      for(let item in data){
          if (item!=='201'){
              s.push(data[item]);
              l.push(item);

          }
      
      }
    

      await this.setState({});
     
    }


    render(){
        return(

            <div>
                <Chart options={this.state.options} series={this.state.series} type="donut" width={850} height={450}
                       style={{
                           //alignItems: 'center',
                           justifyContent: 'center',
                          // marginCenter: 'center',
                           marginInline: 'center',
                           marginRight:20
                       }}/>


            </div>
        )
   } 
   
   
  
}