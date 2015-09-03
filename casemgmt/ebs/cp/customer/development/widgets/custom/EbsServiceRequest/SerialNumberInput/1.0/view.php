<!--
/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  EBS release: 12.1.3
 *  reference: 150202-000157
 *  date: Wed Sep  2 23:11:32 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 03ca879f5a2c2be39ffa18019a7a1fa2b91c3c2c $
 * *********************************************************************************************
 *  File: view.php
 * ****************************************************************************************** */
-->

<? if ($this->data['readOnly']): ?>
    <rn:widget path="output/FieldDisplay" left_justify="true"/>
<? else: ?>
<div id="rn_<?= $this->instanceID ?>" class="<?= $this->classList ?>">
<rn:block id="top"/>
<? if ($this->data['attrs']['label_input']): ?>
    <div id="rn_<?= $this->instanceID ?>_LabelContainer">
        <rn:block id="preLabel"/>
        <label for="rn_<?= $this->instanceID ?>_<?= $this->data['js']['name'] ?>" id="rn_<?= $this->instanceID ?>_Label" class="rn_Label">
        <?= $this->data['attrs']['label_input'] ?>
        <? if ($this->data['attrs']['required']): ?>
            <rn:block id="preRequired"/>
            <span class="rn_Required"> <?= \RightNow\Utils\Config::getMessage((1908)) ?></span><span class="rn_ScreenReaderOnly"> <?= \RightNow\Utils\Config::getMessage((7015)) ?></span>
            <rn:block id="postRequired"/>
        <? endif;
?>
        <? if ($this->data['attrs']['hint']): ?>
            <span class="rn_ScreenReaderOnly"><?= $this->data['attrs']['hint'] ?></span>
        <? endif;
?>
        </label>
        <rn:block id="postLabel">
             <div id="rn_<?= $this->instanceID ?>_ValidationResultDisplay" class="rn_Hidden"></div> 
        </rn:block>
    </div>
<? endif;
?>

<div class="rn_SerialNumberTextInput">
    <? if ($this->data['displayType'] === 'Textarea'): ?>
    <rn:block id="preInput"/>
        <textarea id="rn_<?= $this->instanceID ?>_<?= $this->data['js']['name'] ?>" class="rn_TextArea" rows="7" cols="60" name="<?= $this->data['inputName'] ?>" <?= $this->outputConstraints();
    ?> ><?= $this->data['value'] ?></textarea>
    <rn:block id="postInput"/>
    <? if ($this->data['attrs']['hint'] && $this->data['attrs']['always_show_hint']): ?>
        <rn:block id="preHint"/>
        <span class="rn_HintText"><?= $this->data['attrs']['hint'] ?></span>
        <rn:block id="postHint"/>
    <? endif;
    ?>
    <? else: ?>
    <rn:block id="preInput"/>
        <input type="<?=$this->data['inputType']?>" id="rn_<?=$this->instanceID?>_<?=$this->data['js']['name']?>" name="<?= $this->data['inputName'] ?>" class="rn_<?=$this->data['displayType']?> rn_SerialNumberInput" <?=$this->outputConstraints();?> <?if($this->data['value']
    !== null && $this->data['value'] !== '') echo "value='{$this->data['value']}'";?> />
    <rn:block id="postInput"/>
    <?
    if ($this->data['attrs']['hint'] && $this->data['attrs']['always_show_hint']): ?>
        <rn:block id="preHint"/>
        <span class="rn_HintText"><?= $this->data['attrs']['hint'] ?></span>
        <rn:block id="postHint"/>
    <? endif;
    ?>
        <? if ($this->data['attrs']['require_validation']): ?>
        <div class="rn_TextInputValidate">
            <div id="rn_<?= $this->instanceID ?>_LabelValidateContainer">
                <rn:block id="preValidateLabel"/>
                <label for="rn_<?= $this->instanceID ?>_<?= $this->data['js']['name'] ?>_Validate" id="rn_<?= $this->instanceID ?>_<?= $this->data['js']['name'] ?>_LabelValidate" class="rn_Label"><?printf($this->data['attrs']['label_validation'], $this->data['attrs']['label_input']) ?>
                <? if ($this->data['attrs']['required']): ?>
                    <rn:block id="preValidateRequired"/>
                    <span class="rn_Required"><?= \RightNow\Utils\Config::getMessage((1908)) ?></span><span class="rn_ScreenReaderOnly"> <?= \RightNow\Utils\Config::getMessage((7015)) ?></span>
                    <rn:block id="postValidateRequired"/>
                <? endif;
    ?>
                </label>
                <rn:block id="postValidateLabel"/>
            </div>
            <rn:block id="preValidateInput"/>
            <input type="<?= $this->data['inputType'] ?>" id="rn_<?= $this->instanceID ?>_<?= $this->data['js']['name'] ?>_Validate" name="<?= $this->data['inputName'] ?>_Validation" class="rn_<?=$this->data['displayType']?> rn_Validation" <?= $this->outputConstraints();
    ?> value="<?= $this->data['value'] ?>"/>
            <rn:block id="postValidateInput"/>
        </div>
       <? endif;
    ?>
    <? endif;
    ?>
    <rn:block id="bottom">
        <button class="rn_SerialNumberValidationButton" id="rn_<?= $this->instanceID; ?>_VerifySubmit" type="button">Validate...</button>
        <span id="rn_<?= $this->instanceID; ?>_Loading">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
    </rn:block>
    </div>
</div>
<? endif;
?>