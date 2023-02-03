import React from 'react'
import { useState, useEffect } from 'react'
import { Link, useParams } from 'react-router-dom'//za izvlacenje paramsa iz route-a
import ReqJoinBtn from './ReqJoinBtn'
import { ProgressBar } from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'

function SprintChart(props) {
    const [korisnici, setKorisnici] = useState([])
    const [tasks, setTasks] = useState([])

    useEffect(() => {

        /*
      const request = { 
          method: 'GET',
          headers: { 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
      }


      fetch('https://localhost:7013/Sprint/GetSprintTasks/' + props.sprintID, request).then( response => {
          if(response.ok)
              response.json().then(tasks => {
                  setTasks(tasks)
                  console.log(tasks)
              })
      })*/
      let taskarr = []
      let korarr = []


      console.log(props.tasks)


      props.tasks.forEach((task) => {
        let obj = {id: task.id, ime: task.ime, opis: task.opis, status: task.status}
        obj =   {   ...obj,
                    ...(task.korisnik != null && {korID: task.korisnik.id, korIme: task.korisnik.ime})
                }
        let obj2 = {ime: obj.korIme, id: obj.korID, broj: 0, procenat: 0}
                taskarr.push(obj)
                korarr.push(obj2)
            })


        korarr.forEach((kor) => {
            if(kor.id == null)
            kor.ime = "not yet taken"

            korarr.forEach((kor2) => {
            if(kor.id == kor2.id)
            kor.broj = kor.broj +1;
            
            })
            })
        
        let unikatni = []

        let ukupan = 0

        korarr.forEach((kor) => {
        let br = 0
            unikatni.forEach((kor2) => {
            if(kor.id == kor2.id)
            br = br + 1
            })
        if (br == 0){
        unikatni.push(kor)
    ukupan = ukupan + kor.broj
    }
        })

        unikatni.forEach((kor) =>{
            kor.procenat = (kor.broj / ukupan) * 100
        })

        console.log(unikatni)
        setTasks(taskarr)
        setKorisnici(unikatni)

  }, [])


    return (
        <div style={{padding: "20px", margin:"20px"}}>
      <ProgressBar>

            {
            korisnici.map((kor) => 
                {
                    if(kor.id%4 == 0){
                    return(
                        <ProgressBar striped label={kor.ime} variant="success" now={kor.procenat} key={kor.korID} />
                                )
                    }
                    else if(kor.id%4 == 1){
                        return(
                            <ProgressBar striped label={kor.ime} variant="info" now={kor.procenat} key={kor.korID} />
                                    )
                        }
                    else if(kor.id%4 == 2){
                            return(
                                <ProgressBar striped label={kor.ime} variant="danger" now={kor.procenat} key={kor.korID} />
                                        )
                            }
                    else if(kor.id%4 == 3){
                                return(
                                    <ProgressBar striped label={kor.ime} variant="warning" now={kor.procenat} key={kor.korID} />
                                            )
                                }
                    else if(kor.id == null){
                                    return(
                                        <ProgressBar label={kor.ime} variant="danger" now={kor.procenat} key={kor.korID} />
                                                )
                                    }
                }
            )}
        
    </ProgressBar>
    </div>
    )

}

export default SprintChart