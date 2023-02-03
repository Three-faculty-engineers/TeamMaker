import React from 'react'
import { useEffect, useState } from 'react'
import SprintChart from './SprintChart'


function ChartList(props) {

    const [sprints, setSprints] = useState([])

    useEffect(() => {

        const request = { 
            method: 'GET',
            headers: { 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
        }


        fetch('https://localhost:7013/Sprint/GetSprints/' + props.teamID, request).then( response => {
            if(response.ok)
                response.json().then(sprints => {
                    setSprints(sprints)
                    console.log(sprints)
                })
        })
    }, [])

    return (
        <div className='chartbox'>
            {
            sprints.map((sprint) => 
                {
                    return(
                                <div key="{sprint.id}" className ="chart">
                                    <h4>{sprint.opis}</h4>
                                    <SprintChart teamID = {props.teamID} isLeader = {props.isLeader} sprintID = {sprint.id} tasks = {sprint.taskovi} />
                                </div>
                                )
                }
            )}
        
        </div>
    )
}

export default ChartList